// Benchmark scenario drafted with AI assistance and reviewed by the WJb author.
// Validate results independently before using them for performance claims.

using BenchmarkDotNet.Attributes;
using Hangfire;
using Hangfire.MemoryStorage;
using WJb.Benchmarks.Infrastructure;

namespace WJb.Benchmarks.Enqueue;

[MemoryDiagnoser]
[ShortRunJob]
public class ParallelBenchmarks
{
    [Params(1, 2, 4, 8, 16)]
    public int Workers;

    private const int TotalJobs = 100_000;

    private BackgroundJobClient _jobs = null!;

    [GlobalSetup]
    public void Setup()
    {
        GlobalConfiguration.Configuration
            .UseMemoryStorage();

        _jobs = new BackgroundJobClient();
    }

    [Benchmark]
    public async Task ParallelEnqueue()
    {
        var jobsPerWorker = TotalJobs / Workers;

        var tasks = new Task[Workers];

        for (var worker = 0; worker < Workers; worker++)
        {
            tasks[worker] = Task.Run(() =>
            {
                for (var i = 0; i < jobsPerWorker; i++)
                {
                    _jobs.Enqueue<NoOpJob>(x => x.Execute());
                }

                return Task.CompletedTask;
            });
        }

        await Task.WhenAll(tasks);
    }
}