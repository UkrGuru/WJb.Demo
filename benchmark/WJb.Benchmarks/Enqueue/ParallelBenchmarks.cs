using BenchmarkDotNet.Attributes;
using WJb.Benchmarks.Infrastructure;

namespace WJb.Benchmarks.Enqueue;

[MemoryDiagnoser]
[ShortRunJob]
public class ParallelBenchmarks
{
    [Params(1, 2, 4, 8, 16)]
    public int Workers;

    private const int TotalJobs = 100_000;

    private IWJb _wjb = null!;

    [GlobalSetup]
    public void Setup()
    {
        _wjb = WJbBuilder.Create(
            new InMemoryStore(),
            cfg => cfg.AddAction<NoOpAction>("noop"));
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
                    await _wjb.EnqueueAsync("noop", null);
                }
            });
        }

        await Task.WhenAll(tasks);
    }
}