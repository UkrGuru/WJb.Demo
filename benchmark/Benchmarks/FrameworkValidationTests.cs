using System.Collections.Concurrent;
using Hangfire;
using Hangfire.MemoryStorage;
using Quartz;
using WJb;
using Xunit;

namespace Benchmarks.Tests;

public class FrameworkValidationTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    public async Task All_Frameworks_Should_Execute_All_Jobs(int count)
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

            var trigger = TriggerBuilder.Create()
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(job, trigger);
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
    public async Task WJb_Enqueue_Should_Not_Execute()
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

        Assert.Equal(0, WJbEchoAction.Completed);
    }

    [Fact]
    public Task Hangfire_Enqueue_Should_Not_Execute_Without_Server()
    {
        HangfireEchoJob.Completed = 0;

        var storage = new MemoryStorage();

        var client = new BackgroundJobClient(storage);

        client.Enqueue<HangfireEchoJob>(
            x => x.Execute(new EchoPayload(1, "test")));

        Assert.Equal(0, HangfireEchoJob.Completed);

        return Task.CompletedTask;
    }

    [Fact]
    public async Task Quartz_Enqueue_Should_Not_Execute_Without_Start()
    {
        QuartzEchoJob.Completed = 0;

        var scheduler = await QuartzSchedulerBuilder
            .Create(q => q.UseInMemoryStore())
            .BuildScheduler();

        var job = JobBuilder.Create<QuartzEchoJob>()
            .UsingJobData(new JobDataMap
            {
                ["payload"] = new EchoPayload(1, "test")
            })
            .Build();

        var trigger = TriggerBuilder.Create()
            .StartNow()
            .Build();

        await scheduler.ScheduleJob(job, trigger);

        await Task.Delay(500);

        Assert.Equal(0, QuartzEchoJob.Completed);

        await scheduler.Shutdown();
    }

    [Fact]
    public async Task WJb_Should_Preserve_All_Payloads()
    {
        PayloadCapture.Reset();

        const int count = 100;

        var store = new InMemoryStore();

        var wjb = WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<WJbPayloadCaptureAction>("capture");
        });

        for (int i = 0; i < count; i++)
        {
            await wjb.EnqueueAsync(
                "capture",
                new EchoPayload(i, $"User {i}"));
        }

        await wjb.ExecuteLoopAsync();

        Assert.Equal(count, PayloadCapture.Ids.Count);

        for (int i = 0; i < count; i++)
        {
            Assert.Contains(i, PayloadCapture.Ids);
        }
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

internal static class PayloadCapture
{
    public static ConcurrentBag<int> Ids { get; } = new();

    public static void Reset()
    {
        while (Ids.TryTake(out _))
        {
        }
    }
}

[WJb.ActionName("capture")]
public sealed class WJbPayloadCaptureAction
    : JobAction<EchoPayload?>
{
    public override ValueTask<ActionResult> ExecuteAsync(
        EchoPayload? payload,
        CancellationToken ct = default)
    {
        if (payload is not null)
            PayloadCapture.Ids.Add(payload.Id);

        return ActionResult.Empty;
    }
}