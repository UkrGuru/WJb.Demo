using BenchmarkDotNet.Attributes;
using Quartz;
using Quartz.Impl;

namespace WJb.Benchmarks.Quartz;

[ShortRunJob]
[MemoryDiagnoser]
public class EnqueueBenchmarks
{
    private IScheduler _scheduler = null!;

    [GlobalSetup]
    public async Task Setup()
    {
        var factory = new StdSchedulerFactory();

        _scheduler = await factory.GetScheduler();

        await _scheduler.Start();
    }

    [Benchmark]
    public async Task Quartz_Enqueue()
    {
        var job = JobBuilder
            .Create<NoOpJob>()
            .Build();

        var trigger = TriggerBuilder
            .Create()
            .StartNow()
            .Build();

        await _scheduler.ScheduleJob(job, trigger);
    }

    [GlobalCleanup]
    public async Task Cleanup()
    {
        await _scheduler.Shutdown();
    }
}