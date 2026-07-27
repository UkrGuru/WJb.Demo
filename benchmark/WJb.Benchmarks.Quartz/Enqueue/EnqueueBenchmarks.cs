// Benchmark scenario drafted with AI assistance and reviewed by the WJb author.
// Validate results independently before using them for performance claims.

using BenchmarkDotNet.Attributes;
using Quartz;
using Quartz.Impl;
using WJb.Benchmarks.Infrastructure;

namespace WJb.Benchmarks.Enqueue;

[MemoryDiagnoser]
[ShortRunJob]
public class EnqueueBenchmarks
{
    private IScheduler _scheduler = null!;
    private IJobDetail _job = null!;

    [GlobalSetup]
    public async Task Setup()
    {
        var factory = new StdSchedulerFactory();

        _scheduler = await factory.GetScheduler();

        _job = JobBuilder
            .Create<NoOpJob>()
            .WithIdentity("noop")
            .Build();
    }

    [Benchmark]
    public Task<DateTimeOffset> Enqueue()
    {
        var job = JobBuilder
            .Create<NoOpJob>()
            .WithIdentity(Guid.NewGuid().ToString())
            .Build();

        var trigger = TriggerBuilder
            .Create()
            .StartNow()
            .Build();

        return _scheduler.ScheduleJob(job, trigger);
    }
}