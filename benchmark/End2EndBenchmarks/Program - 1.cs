
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Hangfire;
using Hangfire.MemoryStorage;
using Quartz;

BenchmarkRunner.Run<End2EndTest>();

[SimpleJob]
[MemoryDiagnoser]
public class End2EndTest
{
    [Params(10_000)] //1_000, 10_000, 100_000
    public int Count { get; set; }

    [Benchmark]
    public async Task WJb_End2End_Echo()
    {
        var store = new WJb.InMemoryStore();

        var wjb = WJb.WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<WJbEchoAction>("echo");
        });

        for (var i = 0; i < Count; i++)
        {
            await wjb.EnqueueAsync("echo", new EchoPayload(i, $"User {i}"));
        }

        await wjb.ExecuteLoopAsync();
    }

    [Benchmark]
    public async Task Hangfire_End2End_Echo()
    {
        Interlocked.Exchange(ref HangfireEchoJob.Completed, 0);

        GlobalConfiguration.Configuration
            .UseMemoryStorage();

        using var server = new BackgroundJobServer();

        for (var i = 0; i < Count; i++)
        {
            BackgroundJob.Enqueue<HangfireEchoJob>(
                x => x.Execute(new EchoPayload(i, $"User {i}")));
        }

        while (Volatile.Read(ref HangfireEchoJob.Completed) < Count)
        {
            await Task.Delay(1);
        }
    }
    [Benchmark]
    public async Task Quartz_End2End_Echo()
    {
        var scheduler = await QuartzSchedulerBuilder
            .Create(q => q.UseInMemoryStore())
            .BuildScheduler();

        await scheduler.Start();

        for (var i = 0; i < Count; i++)
        {
            var job = JobBuilder.Create<QuartzEchoJob>()
                .UsingJobData("Id", i)
                .UsingJobData("Name", $"User {i}")
                .WithIdentity($"echo-{i}")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"echo-trigger-{i}")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        await scheduler.Shutdown(waitForJobsToComplete: true);
    }
}
