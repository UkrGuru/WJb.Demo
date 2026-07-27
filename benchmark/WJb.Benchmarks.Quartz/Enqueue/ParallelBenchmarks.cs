// Benchmark scenario drafted with AI assistance and reviewed by the WJb author.
// Validate results independently before using them for performance claims.

using BenchmarkDotNet.Attributes;
using Quartz;
using Quartz.Impl;
using WJb.Benchmarks.Infrastructure;

namespace WJb.Benchmarks.Enqueue;

[MemoryDiagnoser]
[ShortRunJob]
public class ParallelBenchmarks
{
    [Params(1, 2, 4, 8, 16)]
    public int Workers;

    private const int TotalJobs = 100_000;

    private IScheduler _scheduler = null!;

    [GlobalSetup]
    public async Task Setup()
    {
        var factory = new StdSchedulerFactory();

        _scheduler = await factory.GetScheduler();

        await _scheduler.Start();
    }

    [Benchmark]
    public async Task ParallelEnqueue()
    {
        var jobsPerWorker = TotalJobs / Workers;

        var tasks = new Task[Workers];

        for (var worker = 0; worker < Workers; worker++)
        {
            tasks[worker] = Task.Run(async () =>
            {
                for (var i = 0; i < jobsPerWorker; i++)
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
            });
        }

        await Task.WhenAll(tasks);
    }
}