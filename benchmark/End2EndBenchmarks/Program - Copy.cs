using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Quartz;
using Quartz.Impl;
using WJb;

BenchmarkRunner.Run<End2EndTest>();

[SimpleJob]
[MemoryDiagnoser]
public class End2EndTest
{
    [Params(1_000)]
    public int Preloaded { get; set; }

    [Params(100)]
    public int ProcessCount { get; set; }

    private InMemoryStore _store = null!;
    private IWJb _wjb = null!;

    private IScheduler _scheduler = null!;

    [GlobalSetup]
    public void Setup()
    {
        _store = new InMemoryStore();

        _wjb = WJbBuilder.Create(_store, cfg =>
        {
            cfg.AddAction<WJbEchoAction>("echo");
        });

        for (int i = 0; i < Preloaded; i++)
        {
            _wjb.EnqueueAsync(
                "echo",
                new EchoPayload(i, $"Pre {i}")
            ).GetAwaiter().GetResult();
        }

        _scheduler = QuartzSchedulerBuilder
        .Create(q => q.UseInMemoryStore())
        .BuildScheduler()
        .GetAwaiter()
        .GetResult();

        for (int i = 0; i < Preloaded; i++)
        {
            var job = JobBuilder.Create<QuartzEchoJob>()
            .UsingJobData("Id", i)
            .UsingJobData("Name", $"Pre {i}")
            .Build();

            var trigger = TriggerBuilder.Create()
            .StartAt(DateTimeOffset.UtcNow.AddYears(1))
            .Build();

            _scheduler.ScheduleJob(job, trigger)
            .GetAwaiter()
            .GetResult();
        }

        Console.WriteLine($"Completed before start = {QuartzEchoJob.Completed}");

        _scheduler.Start()
        .GetAwaiter()
        .GetResult();

        Console.WriteLine($"Completed after start = {QuartzEchoJob.Completed}");
    }

    [Benchmark]
    public async Task WJb_End2End_Echo()
    {
        for (int i = 0; i < ProcessCount; i++)
        {
            await _wjb.EnqueueAsync(
                "echo",
                new EchoPayload(i, $"User {i}")
            );

            await _wjb.ExecuteOnceAsync();
        }
    }

    [Benchmark]
    public async Task Quartz_End2End_Echo()
    {
        var startCount = Volatile.Read(ref QuartzEchoJob.Completed);

        for (int i = 0; i < ProcessCount; i++)
        {
            var job = JobBuilder.Create<QuartzEchoJob>()
                .UsingJobData("Id", i)
                .UsingJobData("Name", $"User {i}")
                .Build();

            var trigger = TriggerBuilder.Create()
                .StartNow()
                .Build();

            await _scheduler.ScheduleJob(job, trigger);
        }

        while (Volatile.Read(ref QuartzEchoJob.Completed) - startCount < ProcessCount)
        {
            await Task.Yield();
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _wjb.ExecuteLoopAsync().GetAwaiter().GetResult();

        _scheduler.Shutdown(waitForJobsToComplete: true).GetAwaiter().GetResult();
    }
}