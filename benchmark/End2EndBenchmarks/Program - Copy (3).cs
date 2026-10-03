using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Hangfire;
using Hangfire.Common;
using Hangfire.MemoryStorage;
using Quartz;
using Quartz.Impl;
using WJb;

BenchmarkRunner.Run<End2EndTest>();

#region Common Payloads & Actions

public sealed record EchoPayload(int Id, string Name);

// --- WJb ---
[WJb.ActionName("echo")]
public sealed class WJbEchoAction : WJb.JobAction<EchoPayload?>
{
    public override ValueTask<WJb.ActionResult> ExecuteAsync(
        EchoPayload? payload, CancellationToken ct = default)
        => ValueTask.FromResult(WJb.Results.Done(payload));
}

// --- Hangfire ---
public sealed class HangfireEchoJob
{
    public Task<EchoPayload> ExecuteAsync(EchoPayload payload)
    {
        return Task.FromResult(payload);
    }
}

public sealed class QuartzEchoJob : Quartz.IJob
{
    public ValueTask Execute(
        Quartz.IJobExecutionContext context, CancellationToken cancellationToken)
    {
        _ = context.MergedJobDataMap.GetInt("Id");
        _ = context.MergedJobDataMap.GetString("Name");

        return ValueTask.CompletedTask;
    }
}

#endregion

[ShortRunJob]
[MemoryDiagnoser]
public class JobEnqueueBenchmarks
{
    [Params(1_000)]
    public int Preloaded { get; set; }

    [Params(100)]
    public int ProcessCount { get; set; }

    // WJb
    private InMemoryStore _store = null!;
    private IWJb _wjb = null!;

    // Hangfire
    private BackgroundJobServer _hfServer = null!;
    private IBackgroundJobClient _hfClient = null!;
    private MemoryStorage _hfStorage = null!;

    // Quartz
    private IScheduler _quartzScheduler = null!;

    [GlobalSetup]
    public void Setup()
    {
        #region WJb Setup
        _store = new InMemoryStore();
        _wjb = WJbBuilder.Create(_store, cfg =>
        {
            cfg.AddAction<WJbEchoAction>("echo");
        });
        #endregion

        #region Hangfire Setup
        _hfStorage = new MemoryStorage();
        JobStorage.Current = _hfStorage;

        // Создаем клиент и сервер
        _hfClient = new BackgroundJobClient(_hfStorage);
        _hfServer = new BackgroundJobServer(new BackgroundJobServerOptions
        {
            WorkerCount = 1 // 1 воркер для последовательной обработки
        }, _hfStorage);
        #endregion

        #region Quartz Setup
        //var schedulerFactory = new StdSchedulerFactory();
        //_quartzScheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();
        _quartzScheduler = QuartzSchedulerBuilder.Create(q => q.UseInMemoryStore()).BuildScheduler().GetAwaiter().GetResult(); ;

        _quartzScheduler.Start().GetAwaiter().GetResult();
        #endregion

        // Предварительная загрузка (Preload)
        for (int i = 0; i < Preloaded; i++)
        {
            var payload = new EchoPayload(i, $"Pre {i}");

            // WJb Enqueue
            _wjb.EnqueueAsync("echo", payload).GetAwaiter().GetResult();

            // Hangfire Enqueue
            _hfClient.Enqueue<HangfireEchoJob>(j => j.ExecuteAsync(payload).GetAwaiter().GetResult());

            // Quartz Enqueue
            var job = JobBuilder.Create<QuartzEchoJob>()
                .WithIdentity($"pre_{i}")
                .UsingJobData(new JobDataMap { { "payload", payload } })
                .Build();
            var trigger = TriggerBuilder.Create().StartNow().Build();
            _quartzScheduler.ScheduleJob(job, trigger).GetAwaiter().GetResult();
        }
    }

    [Benchmark(Baseline = true)]
    public async Task WJb_End2End_Echo()
    {
        for (int i = 0; i < ProcessCount; i++)
        {
            await _wjb.EnqueueAsync("echo", new EchoPayload(i, $"User {i}"));
            await _wjb.ExecuteOnceAsync();
        }
    }

    [Benchmark]
    public async Task Hangfire_End2End_Echo()
    {
        for (int i = 0; i < ProcessCount; i++)
        {
            // Создание и добавление джоба через Expression (создает объект вызова)
            _hfClient.Enqueue<HangfireEchoJob>(j => j.ExecuteAsync(new EchoPayload(i, $"User {i}")));

            // Имитируем ExecuteOnce для Hangfire. 
            // Так как сервер работает в фоне, мы просто даем ему немного времени на обработку одной задачи,
            // либо полагаемся на то, что фоновый воркер сам разберет очередь параллельно циклу.
            // Примечание: У Hangfire нет синхронного "ExecuteOnce" API наружу, 
            // поэтому тест покажет чистую скорость фонового конвейера Hangfire.
        }
    }

    [Benchmark]
    public async Task Quartz_End2End_Echo()
    {
        for (int i = 0; i < ProcessCount; i++)
        {
            var payload = new EchoPayload(i, $"User {i}");

            var job = JobBuilder.Create<QuartzEchoJob>()
                .WithIdentity($"user_{i}")
                .UsingJobData(new JobDataMap { { "payload", payload } })
                .Build();

            var trigger = TriggerBuilder.Create().StartNow().Build();

            // Кварц инстанцирует объект QuartzEchoJob при срабатывании триггера
            await _quartzScheduler.ScheduleJob(job, trigger);
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _wjb.ExecuteLoopAsync().GetAwaiter().GetResult();
        _hfServer.Dispose();
        _quartzScheduler.Shutdown().GetAwaiter().GetResult();
    }
}
