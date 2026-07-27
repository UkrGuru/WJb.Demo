using BenchmarkDotNet.Attributes;
using Quartz;
using Quartz.Impl;

namespace WJb.Benchmarks.Quartz;

[ShortRunJob]
[MemoryDiagnoser]
public class EnqueueManyBenchmarks
{
    [Params(1000, 10000, 100000)]
    public int Count;

    private IScheduler _scheduler = null!;

    [GlobalSetup]
    public async Task Setup()
    {
        var factory = new StdSchedulerFactory();

        _scheduler = await factory.GetScheduler();

        await _scheduler.Start();
    }

    [Benchmark]
    public async Task Quartz_EnqueueMany()
    {
        for (var i = 0; i < Count; i++)
        {
            var job = JobBuilder
                .Create<NoOpJob>()
                .WithIdentity(Guid.NewGuid().ToString())
                .Build();

            var trigger = TriggerBuilder
                .Create()
                .StartNow()
                .Build();

            await _scheduler.ScheduleJob(job, trigger);
        }
    }

    [GlobalCleanup]
    public async Task Cleanup()
    {
        await _scheduler.Shutdown();
    }
}

public sealed class NoOpJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        return Task.CompletedTask;
    }
}