using Hangfire;
using Hangfire.MemoryStorage;
using Quartz;
using WJb;
using Xunit;

public class FrameworkValidationTests
{
    [Fact]
    public async Task All_Frameworks_Should_Execute_100_Jobs()
    {
        const int count = 100;

        WJbEchoAction.Completed = 0;
        HangfireEchoJob.Completed = 0;
        QuartzEchoJob.Completed = 0;

        // setup как в benchmark
        var store = new InMemoryStore();

        var wjb = WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<WJbEchoAction>("echo");
        });

        var hfStorage = new MemoryStorage();

        using var hfServer = new BackgroundJobServer(
            new BackgroundJobServerOptions
            {
                WorkerCount = 1
            },
            hfStorage);

        var hfClient = new BackgroundJobClient(hfStorage);

        var scheduler = await QuartzSchedulerBuilder
            .Create(q => q.UseInMemoryStore())
            .BuildScheduler();

        await scheduler.Start();

        // enqueue
        for (int i = 0; i < count; i++)
        {
            var payload = new EchoPayload(i, $"User {i}");

            await wjb.EnqueueAsync("echo", payload);

            hfClient.Enqueue<HangfireEchoJob>(
                x => x.Execute(payload));

            var job = JobBuilder.Create<QuartzEchoJob>()
                .WithIdentity($"job_{i}")
                .UsingJobData(new JobDataMap
                {
                    { "payload", payload }
                })
                .Build();

            var trigger = TriggerBuilder.Create()
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        // выполнить WJb
        await wjb.ExecuteLoopAsync();

        // дождаться Hangfire и Quartz
        var timeout = DateTime.UtcNow.AddSeconds(10);

        while (DateTime.UtcNow < timeout)
        {
            if (HangfireEchoJob.Completed >= count &&
                QuartzEchoJob.Completed >= count)
            {
                break;
            }

            await Task.Delay(100);
        }

        Assert.Equal(count, WJbEchoAction.Completed);
        Assert.Equal(count, HangfireEchoJob.Completed);
        Assert.Equal(count, QuartzEchoJob.Completed);

        await scheduler.Shutdown();
    }
}