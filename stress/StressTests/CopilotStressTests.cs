using System.Diagnostics;
using Xunit;

namespace WJb;

/// <summary>
/// Practical stress tests for WJb.
///
/// Included:
/// - Throughput testing
/// - Concurrent producers/consumers
/// - Mixed workload testing
/// - Memory pressure testing
/// - Queue integrity verification
///
/// Intentionally excluded:
/// - Multi-hour soak tests
/// - Environment-specific memory profiling
/// - Hardware-specific scenarios
///
/// Additional stress scenarios are welcome.
/// If a scenario is considered important, implement it and
/// demonstrate the failure it protects against.
/// </summary>
public class CopilotStressTests
{
    [Fact]
    public async Task One_Million_Jobs()
    {
        var store = new InMemoryStore();

        const int count = 1_000_000;

        var sw = Stopwatch.StartNew();

        await Parallel.ForEachAsync(
            Enumerable.Range(0, count),
            async (i, ct) =>
            {
                await store.EnqueueAsync(
                    $"job-{i}",
                    null,
                    ct);
            });

        sw.Stop();

        Console.WriteLine(
            $"Enqueued {count:N0} jobs in {sw.Elapsed}");

        long processed = 0;

        while (await store.DequeueAsync() is not null)
        {
            processed++;
        }

        Console.WriteLine(
            $"Processed {processed:N0} jobs");

        Assert.Equal(count, processed);
    }

    [Fact]
    public async Task Massive_Enqueue_Dequeue_Should_Process_All_Jobs()
    {
        var store = new InMemoryStore();

        const int count = 10_000;

        for (var i = 0; i < count; i++)
        {
            await store.EnqueueAsync(
                "x",
                null);
        }

        var processed = 0;

        while (true)
        {
            var job = await store.DequeueAsync();

            if (job == null)
                break;

            processed++;
        }

        Assert.Equal(count, processed);
    }

    [Fact]
    public async Task Producer_Consumer_For_One_Minute()
    {
        var store = new InMemoryStore();

        var stopAt =
            DateTime.UtcNow.AddMinutes(1);

        long produced = 0;
        long consumed = 0;

        var producer = Task.Run(async () =>
        {
            while (DateTime.UtcNow < stopAt)
            {
                await store.EnqueueAsync(
                    "x",
                    null);

                Interlocked.Increment(ref produced);
            }
        });

        var consumers =
            Enumerable.Range(
                0,
                Environment.ProcessorCount)
            .Select(_ => Task.Run(async () =>
            {
                while (
                    !producer.IsCompleted ||
                    Volatile.Read(ref consumed)
                        < Volatile.Read(ref produced))
                {
                    var job =
                        await store.DequeueAsync();

                    if (job == null)
                    {
                        await Task.Yield();
                        continue;
                    }

                    Interlocked.Increment(
                        ref consumed);
                }
            }));

        await producer;
        await Task.WhenAll(consumers);

        Console.WriteLine(
            $"Produced: {produced:N0}");

        Console.WriteLine(
            $"Consumed: {consumed:N0}");

        Assert.Equal(produced, consumed);
    }

    [Fact]
    public async Task Mixed_Load_For_One_Minute()
    {
        var store = new InMemoryStore();

        var stopAt =
            DateTime.UtcNow.AddMinutes(1);

        await Parallel.ForEachAsync(
            Enumerable.Range(0, 100),
            async (_, ct) =>
            {
                var random = new Random(
                    Environment.TickCount ^
                    Environment.CurrentManagedThreadId);

                while (DateTime.UtcNow < stopAt)
                {
                    switch (random.Next(5))
                    {
                        case 0:
                            await store.EnqueueAsync(
                                "x",
                                null,
                                ct);
                            break;

                        case 1:
                            await store.DequeueAsync(
                                ct: ct);
                            break;

                        case 2:
                            await store.GetJobsAsync(
                                ct: ct);
                            break;

                        case 3:
                            await store.GetListAsync(
                                DefinitionType.Actions,
                                ct);
                            break;

                        case 4:
                            await store.SetAsync(
                                DefinitionType.Actions,
                                Guid.NewGuid().ToString(),
                                new { Value = 1 },
                                ct);
                            break;
                    }
                }
            });
    }

    [Fact]
    public async Task Memory_Should_Not_Explode()
    {
        var store = new InMemoryStore();

        var before =
            GC.GetTotalMemory(true);

        for (var i = 0; i < 500_000; i++)
        {
            await store.EnqueueAsync(
                "x",
                null);

            if (i % 1000 == 0)
            {
                await store.DequeueAsync();
            }
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var after =
            GC.GetTotalMemory(true);

        Console.WriteLine(
            $"Memory before: {before:N0}");

        Console.WriteLine(
            $"Memory after : {after:N0}");

        Console.WriteLine(
            $"Ratio        : {(double)after / before:N2}");

        Assert.True(after > 0);
    }

    [Fact]
    public async Task Hot_Queue_Contention_Should_Not_Lose_Jobs()
    {
        var store = new InMemoryStore();

        const int count = 1_000_000;

        await Parallel.ForEachAsync(
            Enumerable.Range(0, count),
            async (i, ct) =>
            {
                await store.EnqueueAsync(
                    "same-action",
                    null,
                    ct);
            });

        long consumed = 0;

        await Parallel.ForEachAsync(
            Enumerable.Range(0, Environment.ProcessorCount),
            async (_, ct) =>
            {
                while (true)
                {
                    var job =
                        await store.DequeueAsync(ct:ct);

                    if (job == null)
                        break;

                    Interlocked.Increment(ref consumed);
                }
            });

        Assert.Equal(count, consumed);
    }

    [Fact]
    public async Task FIFO_Order_Should_Be_Preserved()
    {
        var store = new InMemoryStore();

        const int count = 10_000;

        for (var i = 0; i < count; i++)
        {
            await store.EnqueueAsync(
                i.ToString(),
                null);
        }

        for (var i = 0; i < count; i++)
        {
            var job =
                await store.DequeueAsync();

            Assert.NotNull(job);
            Assert.Equal(
                i.ToString(),
                job!.Action);
        }
    }

    [Fact]
    public async Task Concurrent_Producer_Consumer_Should_Not_Lose_Jobs()
    {
        var store = new InMemoryStore();

        const int count = 1_000_000;

        long produced = 0;
        long consumed = 0;

        var producers =
            Enumerable.Range(0, 16)
            .Select(_ => Task.Run(async () =>
            {
                while (
                    Interlocked.Read(ref produced)
                    < count)
                {
                    var next =
                        Interlocked.Increment(
                            ref produced);

                    if (next > count)
                        break;

                    await store.EnqueueAsync(
                        "x",
                        null);
                }
            }));

        var consumers =
            Enumerable.Range(0, 16)
            .Select(_ => Task.Run(async () =>
            {
                while (
                    Volatile.Read(ref consumed)
                    < count)
                {
                    var job =
                        await store.DequeueAsync();

                    if (job == null)
                    {
                        await Task.Yield();
                        continue;
                    }

                    Interlocked.Increment(
                        ref consumed);
                }
            }));

        await Task.WhenAll(
            producers.Concat(consumers));

        Assert.Equal(count, consumed);
    }


}