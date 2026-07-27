using BenchmarkDotNet.Attributes;
using Hangfire;
using Hangfire.MemoryStorage;

namespace WJb.Benchmarks.Hangfire;

[ShortRunJob]
[MemoryDiagnoser]
public class EnqueueManyBenchmarks
{
    [Params(1000, 10000, 100000)]
    public int Count;

    [GlobalSetup]
    public void Setup()
    {
        GlobalConfiguration.Configuration
            .UseMemoryStorage();
    }

    [Benchmark]
    public void Hangfire_EnqueueMany()
    {
        for (var i = 0; i < Count; i++)
        {
            BackgroundJob.Enqueue<NoOpJob>(
                x => x.ExecuteAsync());
        }
    }
}

