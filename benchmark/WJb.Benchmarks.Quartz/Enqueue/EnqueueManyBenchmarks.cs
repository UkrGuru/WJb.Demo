// Benchmark scenario drafted with AI assistance and reviewed by the WJb author.
// Validate results independently before using them for performance claims.

using BenchmarkDotNet.Attributes;
using Quartz;
using Quartz.Impl;
using WJb.Benchmarks.Infrastructure;

namespace WJb.Benchmarks.Enqueue;

[MemoryDiagnoser]
[ShortRunJob]
public class EnqueueManyBenchmarks
{
    [Params(1_000, 10_000, 100_000)]
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
    public async Task EnqueueMany()
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
}