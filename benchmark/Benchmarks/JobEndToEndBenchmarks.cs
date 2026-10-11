namespace Benchmarks;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Hangfire;
using Hangfire.MemoryStorage;
using Quartz;
using WJb;

[SimpleJob(RuntimeMoniker.Net10_0)]
[MemoryDiagnoser]
public class JobEndToEndBenchmarks
{
    [Params(100, 1000, 10000)]
    public int JobCount { get; set; }

    private InMemoryStore _store = null!;
    private IWJb _wjb = null!;

    private MemoryStorage _hfStorage = null!;
    private BackgroundJobServer _hfServer = null!;
    private IBackgroundJobClient _hfClient = null!;

    private IScheduler _quartzScheduler = null!;

    [IterationSetup]
    public void IterationSetup()
    {
        WJbEchoAction.Completed = 0;
        HangfireEchoJob.Completed = 0;
        QuartzEchoJob.Completed = 0;

        _store = new InMemoryStore();

        _wjb = WJbBuilder.Create(_store, cfg =>
        {
            cfg.AddAction<WJbEchoAction>("echo");
        });

        _hfStorage = new MemoryStorage();

        _hfClient = new BackgroundJobClient(_hfStorage);

        _hfServer = new BackgroundJobServer(
            new BackgroundJobServerOptions
            {
                WorkerCount = 1
            },
            _hfStorage);

        _quartzScheduler = QuartzSchedulerBuilder
            .Create(q =>
            {
                q.UseInMemoryStore();

                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 1;
                });
            })
            .BuildScheduler()
            .GetAwaiter()
            .GetResult();

        _quartzScheduler.Start()
            .GetAwaiter()
            .GetResult();
    }

    [Benchmark(Baseline = true)]
    public async Task WJb_EndToEnd()
    {
        for (int i = 0; i < JobCount; i++)
        {
            await _wjb.EnqueueAsync("echo", new EchoPayload(i, $"User {i}"));
        }

        await _wjb.ExecuteLoopAsync();

        if (WJbEchoAction.Completed != JobCount)
        {
            throw new InvalidOperationException(
                $"Expected={JobCount}, Actual={WJbEchoAction.Completed}");
        }
    }

    [Benchmark]
    public Task Hangfire_EndToEnd()
    {
        for (int i = 0; i < JobCount; i++)
        {
            _hfClient.Enqueue<HangfireEchoJob>(
                x => x.Execute(
                    new EchoPayload(i, $"User {i}")));
        }

        if (!SpinWait.SpinUntil(
                () => HangfireEchoJob.Completed >= JobCount,
                TimeSpan.FromSeconds(30)))
        {
            throw new TimeoutException(
                $"Expected={JobCount}, Actual={HangfireEchoJob.Completed}");
        }

        if (HangfireEchoJob.Completed != JobCount)
        {
            throw new InvalidOperationException(
                $"Expected={JobCount}, Actual={HangfireEchoJob.Completed}");
        }

        return Task.CompletedTask;
    }

    [Benchmark]
    public async Task Quartz_EndToEnd()
    {
        for (int i = 0; i < JobCount; i++)
        {
            var payload =
                new EchoPayload(i, $"User {i}");

            var job = JobBuilder.Create<QuartzEchoJob>()
                .WithIdentity($"job-{i}")
                .UsingJobData(new JobDataMap
                {
                    ["payload"] = payload
                })
                .Build();

            var trigger = TriggerBuilder.Create()
                .StartNow()
                .Build();

            await _quartzScheduler.ScheduleJob(
                job,
                trigger);
        }

        if (!SpinWait.SpinUntil(
                () => QuartzEchoJob.Completed >= JobCount,
                TimeSpan.FromSeconds(30)))
        {
            throw new TimeoutException(
                $"Expected={JobCount}, Actual={QuartzEchoJob.Completed}");
        }

        if (QuartzEchoJob.Completed != JobCount)
        {
            throw new InvalidOperationException(
                $"Expected={JobCount}, Actual={QuartzEchoJob.Completed}");
        }
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        _hfServer.Dispose();

        _quartzScheduler
            .Shutdown(waitForJobsToComplete: true)
            .GetAwaiter()
            .GetResult();
    }
}