
using Hangfire;
using Hangfire.MemoryStorage;
using Quartz;
using WJb;
using Xunit;

namespace Benchmarks.Tests;

public class EndToEndValidationTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    public async Task All_Frameworks_Should_Execute_All_Jobs_EndToEnd(int count)
    {
        WJbEchoAction.Completed = 0;
        HangfireEchoJob.Completed = 0;
        QuartzEchoJob.Completed = 0;

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
                    ["payload"] = payload
                })
                .Build();

            await scheduler.ScheduleJob(
                job,
                TriggerBuilder.Create().StartNow().Build());
        }

        await wjb.ExecuteLoopAsync();

        await WaitForAsync(
            () =>
                HangfireEchoJob.Completed >= count &&
                QuartzEchoJob.Completed >= count);

        Assert.Equal(count, WJbEchoAction.Completed);
        Assert.Equal(count, HangfireEchoJob.Completed);
        Assert.Equal(count, QuartzEchoJob.Completed);

        await scheduler.Shutdown();
    }

    [Fact]
    public async Task WJb_EndToEnd_Should_Execute_Single_Job()
    {
        WJbEchoAction.Completed = 0;

        var store = new InMemoryStore();

        var wjb = WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<WJbEchoAction>("echo");
        });

        await wjb.EnqueueAsync(
            "echo",
            new EchoPayload(1, "test"));

        await wjb.ExecuteLoopAsync();

        Assert.Equal(1, WJbEchoAction.Completed);
    }

    [Fact]
    public async Task Hangfire_EndToEnd_Should_Execute_Single_Job()
    {
        HangfireEchoJob.Completed = 0;

        var storage = new MemoryStorage();

        using var server = new BackgroundJobServer(
            new BackgroundJobServerOptions
            {
                WorkerCount = 1
            },
            storage);

        var client = new BackgroundJobClient(storage);

        client.Enqueue<HangfireEchoJob>(
            x => x.Execute(new EchoPayload(1, "test")));

        await WaitForAsync(
            () => HangfireEchoJob.Completed == 1);

        Assert.Equal(1, HangfireEchoJob.Completed);
    }

    [Fact]
    public async Task Quartz_EndToEnd_Should_Execute_Single_Job()
    {
        QuartzEchoJob.Completed = 0;

        var scheduler = await QuartzSchedulerBuilder
            .Create(q => q.UseInMemoryStore())
            .BuildScheduler();

        await scheduler.Start();

        var job = JobBuilder.Create<QuartzEchoJob>()
            .UsingJobData(new JobDataMap
            {
                ["payload"] = new EchoPayload(1, "test")
            })
            .Build();

        await scheduler.ScheduleJob(
            job,
            TriggerBuilder.Create().StartNow().Build());

        await WaitForAsync(
            () => QuartzEchoJob.Completed == 1);

        Assert.Equal(1, QuartzEchoJob.Completed);

        await scheduler.Shutdown();
    }

    [Fact]
    public async Task WJb_EndToEnd_Should_Execute_Exactly_Once()
    {
        WJbEchoAction.Completed = 0;

        var store = new InMemoryStore();

        var wjb = WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<WJbEchoAction>("echo");
        });

        await wjb.EnqueueAsync(
            "echo",
            new EchoPayload(1, "test"));

        await wjb.ExecuteLoopAsync();

        Assert.Equal(1, WJbEchoAction.Completed);
    }

    [Fact]
    public async Task Hangfire_EndToEnd_Should_Execute_Exactly_Once()
    {
        HangfireEchoJob.Completed = 0;

        var storage = new MemoryStorage();

        using var server = new BackgroundJobServer(
            new BackgroundJobServerOptions
            {
                WorkerCount = 1
            },
            storage);

        var client = new BackgroundJobClient(storage);

        client.Enqueue<HangfireEchoJob>(
            x => x.Execute(new EchoPayload(1, "test")));

        await WaitForAsync(
            () => HangfireEchoJob.Completed == 1);

        await Task.Delay(500);

        Assert.Equal(1, HangfireEchoJob.Completed);
    }

    [Fact]
    public async Task Quartz_EndToEnd_Should_Execute_Exactly_Once()
    {
        QuartzEchoJob.Completed = 0;

        var scheduler = await QuartzSchedulerBuilder
            .Create(q => q.UseInMemoryStore())
            .BuildScheduler();

        await scheduler.Start();

        var job = JobBuilder.Create<QuartzEchoJob>()
            .UsingJobData(new JobDataMap
            {
                ["payload"] = new EchoPayload(1, "test")
            })
            .Build();

        await scheduler.ScheduleJob(
            job,
            TriggerBuilder.Create().StartNow().Build());

        await WaitForAsync(
            () => QuartzEchoJob.Completed == 1);

        await Task.Delay(500);

        Assert.Equal(1, QuartzEchoJob.Completed);

        await scheduler.Shutdown();
    }

    private static async Task WaitForAsync(
        Func<bool> predicate,
        int timeoutSeconds = 30)
    {
        var timeout = DateTime.UtcNow.AddSeconds(timeoutSeconds);

        while (DateTime.UtcNow < timeout)
        {
            if (predicate())
                return;

            await Task.Delay(50);
        }

        throw new TimeoutException(
            $"Condition was not satisfied within {timeoutSeconds} seconds.");
    }
}