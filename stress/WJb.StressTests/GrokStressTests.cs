using System.Diagnostics;
using Xunit;

namespace WJb;

public class GrokStressTests
{
    private readonly ITestOutputHelper _output;

    public GrokStressTests(ITestOutputHelper output)
    {
        _output = output;
    }

    // ============================================================
    // 1. The most important test: real competition
    // ============================================================
    [Fact]
    public async Task Concurrent_Producer_Consumer_High_Load()
    {
        var store = new InMemoryStore();
        const int totalJobs = 2_000_000;
        const int producers = 8;
        int consumers = Math.Max(4, Environment.ProcessorCount);

        long produced = 0;
        long consumed = 0;

        var cts = new CancellationTokenSource();
        var sw = Stopwatch.StartNew();

        // Producers
        var producerTasks = Enumerable.Range(0, producers).Select(_ => Task.Run(async () =>
        {
            while (true)
            {
                var current = Interlocked.Increment(ref produced);
                if (current > totalJobs)
                {
                    Interlocked.Decrement(ref produced);
                    break;
                }

                await store.EnqueueAsync($"job-{current}", null, cts.Token);
            }
        }, cts.Token)).ToArray();

        // Consumers
        var consumerTasks = Enumerable.Range(0, consumers).Select(_ => Task.Run(async () =>
        {
            while (Volatile.Read(ref consumed) < totalJobs)
            {
                var job = await store.DequeueAsync(ct:cts.Token);

                if (job is null)
                {
                    await Task.Yield();
                    continue;
                }

                Interlocked.Increment(ref consumed);
            }
        }, cts.Token)).ToArray();

        await Task.WhenAll(producerTasks);
        await Task.WhenAll(consumerTasks);

        sw.Stop();

        _output.WriteLine($"Produced : {produced:N0}");
        _output.WriteLine($"Consumed : {consumed:N0}");
        _output.WriteLine($"Time     : {sw.Elapsed}");
        _output.WriteLine($"Throughput: {totalJobs / sw.Elapsed.TotalSeconds:N0} jobs/sec");

        Assert.Equal(totalJobs, produced);
        Assert.Equal(totalJobs, consumed);
    }

    // ============================================================
    // 2. Rigorous test for memory leaks
    // ============================================================
    [Fact]
    public async Task Memory_Pressure_Fill_And_Drain_Multiple_Cycles()
    {
        const int jobsPerCycle = 1_000_000;
        const int cycles = 5;

        long? baseline = null;
        long previous = 0;

        for (int cycle = 1; cycle <= cycles; cycle++)
        {
            var store = new InMemoryStore();   // ← a new store every cycle

            await Parallel.ForEachAsync(
                Enumerable.Range(0, jobsPerCycle),
                async (i, ct) => await store.EnqueueAsync($"c{cycle}-{i}", null, ct));

            long drained = 0;
            while (await store.DequeueAsync() is not null)
                drained++;

            Assert.Equal(jobsPerCycle, drained);

            ForceGc();
            long after = GC.GetTotalMemory(true);

            if (baseline is null)
            {
                baseline = after;
                previous = after;
            }

            double ratioFromBaseline = (double)after / baseline.Value;
            double ratioFromPrevious = (double)after / previous;

            _output.WriteLine(
                $"Cycle {cycle}: {after:N0} bytes | ×{ratioFromBaseline:N2} base | ×{ratioFromPrevious:N2} prev");

            if (cycle > 1)
            {
                // Now the growth should be minimal
                Assert.True(ratioFromPrevious < 1.3,
                    $"Memory keeps growing after cycle {cycle}: ×{ratioFromPrevious:N2}");
            }

            previous = after;
        }
    }
    // ============================================================
    // 3. Producer-Consumer over a prolonged period
    // ============================================================
    [Fact]
    public async Task Sustained_Load_For_Two_Minutes()
    {
        var store = new InMemoryStore();
        var stopAt = DateTime.UtcNow.AddMinutes(2);

        long produced = 0;
        long consumed = 0;

        var producer = Task.Run(async () =>
        {
            while (DateTime.UtcNow < stopAt)
            {
                await store.EnqueueAsync("x", null);
                Interlocked.Increment(ref produced);
            }
        });

        var consumers = Enumerable.Range(0, Environment.ProcessorCount)
            .Select(_ => Task.Run(async () =>
            {
                while (DateTime.UtcNow < stopAt ||
                       Volatile.Read(ref consumed) < Volatile.Read(ref produced))
                {
                    var job = await store.DequeueAsync();

                    if (job is null)
                    {
                        await Task.Delay(1);
                        continue;
                    }

                    Interlocked.Increment(ref consumed);
                }
            }))
            .ToArray();

        await producer;
        await Task.WhenAll(consumers);

        _output.WriteLine($"Produced: {produced:N0}");
        _output.WriteLine($"Consumed: {consumed:N0}");
        _output.WriteLine($"Difference: {produced - consumed}");

        Assert.Equal(produced, consumed);
    }

    // ============================================================
    // 4. Mixed load + integrity check
    // ============================================================
    [Fact]
    public async Task Mixed_Operations_Under_Heavy_Load()
    {
        var store = new InMemoryStore();
        var stopAt = DateTime.UtcNow.AddMinutes(2);

        long enqueued = 0;
        long dequeued = 0;
        long errors = 0;

        await Parallel.ForEachAsync(
            Enumerable.Range(0, Environment.ProcessorCount * 2),
            async (_, ct) =>
            {
                var rnd = new Random(Guid.NewGuid().GetHashCode());

                while (DateTime.UtcNow < stopAt)
                {
                    try
                    {
                        switch (rnd.Next(6))
                        {
                            case 0:
                            case 1:
                                await store.EnqueueAsync("mixed", null, ct);
                                Interlocked.Increment(ref enqueued);
                                break;

                            case 2:
                            case 3:
                                if (await store.DequeueAsync(ct:ct) is not null)
                                    Interlocked.Increment(ref dequeued);
                                break;

                            case 4:
                                await store.GetJobsAsync(ct:ct);
                                break;

                            case 5:
                                await store.SetAsync(
                                    DefinitionType.Actions,
                                    Guid.NewGuid().ToString("N"),
                                    new { Ts = DateTime.UtcNow },
                                    ct);
                                break;
                        }
                    }
                    catch
                    {
                        Interlocked.Increment(ref errors);
                    }
                }
            });

        // Clear the queue
        while (await store.DequeueAsync() is not null)
        {
            Interlocked.Increment(ref dequeued);
        }

        _output.WriteLine($"Enqueued : {enqueued:N0}");
        _output.WriteLine($"Dequeued : {dequeued:N0}");
        _output.WriteLine($"Errors   : {errors}");

        Assert.Equal(0, errors);
        Assert.True(dequeued <= enqueued);
    }

    // ============================================================
    // 5. Cancellation must not break the store.
    // ============================================================
    [Fact]
    public async Task Cancellation_Does_Not_Corrupt_Store()
    {
        var store = new InMemoryStore();
        var cts = new CancellationTokenSource();

        var enqueueTask = Parallel.ForEachAsync(
            Enumerable.Range(0, 500_000),
            new ParallelOptions { CancellationToken = cts.Token },
            async (i, ct) =>
            {
                await store.EnqueueAsync($"cancel-{i}", null, ct);
            });

        // We're cancelling at roughly the halfway point.
        await Task.Delay(300);
        cts.Cancel();

        try
        {
            await enqueueTask;
        }
        catch (OperationCanceledException)
        {
            // As expected
        }

        // The store must remain in a consistent state.
        long count = 0;
        while (await store.DequeueAsync() is not null)
        {
            count++;
        }

        _output.WriteLine($"Jobs that survived cancellation: {count:N0}");
        Assert.True(count > 0);
        Assert.True(count < 500_000);
    }

    // ============================================================
    // 6. Very large volume (optional, can be marked with [Trait])
    // ============================================================
    [Fact]
    public async Task Five_Million_Jobs_Fully_Concurrent()
    {
        var store = new InMemoryStore();
        const int count = 5_000_000;

        var sw = Stopwatch.StartNew();

        // Parallel enqueue
        await Parallel.ForEachAsync(
            Enumerable.Range(0, count),
            async (i, ct) => await store.EnqueueAsync($"big-{i}", null, ct));

        // Parallel dequeue
        long processed = 0;
        await Parallel.ForEachAsync(
            Enumerable.Range(0, Environment.ProcessorCount),
            async (_, ct) =>
            {
                while (true)
                {
                    var job = await store.DequeueAsync(ct:ct);
                    if (job is null) break;
                    Interlocked.Increment(ref processed);
                }
            });

        sw.Stop();

        _output.WriteLine($"5M jobs processed in {sw.Elapsed}");
        Assert.Equal(count, processed);
    }

    // ========== Helpers ==========

    private static void ForceGc()
    {
        GC.Collect(2, GCCollectionMode.Forced, blocking: true, compacting: true);
        GC.WaitForPendingFinalizers();
        GC.Collect(2, GCCollectionMode.Forced, blocking: true, compacting: true);
    }
}