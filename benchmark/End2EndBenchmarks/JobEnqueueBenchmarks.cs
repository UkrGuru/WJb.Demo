using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Hangfire;
using Hangfire.MemoryStorage;
using Quartz;
using WJb;

//[ShortRunJob]
[SimpleJob(RuntimeMoniker.Net10_0)]
[MemoryDiagnoser]
public partial class JobEnqueueBenchmarks
{
    [Params(1, 100, 1000, 10000)]
    public int JobCount { get; set; }

    private InMemoryStore _store = null!;
    private IWJb _wjb = null!;

    private BackgroundJobServer _hfServer = null!;
    private IBackgroundJobClient _hfClient = null!;
    private MemoryStorage _hfStorage = null!;

    private IScheduler _quartzScheduler = null!;

    [IterationSetup]
    public void IterationSetup()
    {
        WJbEchoAction.Completed = 0;
        HangfireEchoJob.Completed = 0;
        QuartzEchoJob.Completed = 0;
    }

    [GlobalSetup]
    public void Setup()
    {
        _store = new InMemoryStore();

        _wjb = WJbBuilder.Create(_store, cfg =>
        {
            cfg.AddAction<WJbEchoAction>("echo");
        });

        _hfStorage = new MemoryStorage();
        JobStorage.Current = _hfStorage;

        _hfClient = new BackgroundJobClient(_hfStorage);

        _hfServer = new BackgroundJobServer(
            new BackgroundJobServerOptions { WorkerCount = 1 },
            _hfStorage);

        _quartzScheduler = QuartzSchedulerBuilder
            .Create(q => q.UseInMemoryStore())
            .BuildScheduler()
            .GetAwaiter()
            .GetResult();

        _quartzScheduler.Start()
            .GetAwaiter()
            .GetResult();
    }


    [Benchmark(Baseline = true)]
    public async Task WJb_Enqueue()
    {
        for (int i = 0; i < JobCount; i++)
            await _wjb.EnqueueAsync("echo", new EchoPayload(i, $"User {i}"));
    }

    [Benchmark]
    public Task Hangfire_Enqueue()
    {
        for (int i = 0; i < JobCount; i++)
            _hfClient.Enqueue<HangfireEchoJob>(
                j => j.Execute(new EchoPayload(i, $"User {i}")));

        return Task.CompletedTask;
    }

    [Benchmark]
    public async Task Quartz_Enqueue()
    {
        for (int i = 0; i < JobCount; i++)
        {
            var payload = new EchoPayload(i, $"User {i}");

            var job = JobBuilder.Create<QuartzEchoJob>()
                .UsingJobData(new JobDataMap
                {
                { "payload", payload }
                })
                .Build();

            var trigger = TriggerBuilder.Create().StartNow().Build();

            await _quartzScheduler.ScheduleJob(job, trigger);
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _wjb.ExecuteLoopAsync().GetAwaiter().GetResult();

        Thread.Sleep(1000);

        Console.WriteLine(
            $"Completed: WJb={WJbEchoAction.Completed}, " +
            $"Hangfire={HangfireEchoJob.Completed}, " +
            $"Quartz={QuartzEchoJob.Completed}");

        _hfServer.Dispose();
        _quartzScheduler.Shutdown().GetAwaiter().GetResult();
    }
}
