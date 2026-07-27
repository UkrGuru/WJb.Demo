using BenchmarkDotNet.Attributes;
using Hangfire;
using Hangfire.MemoryStorage;

namespace WJb.Benchmarks.Hangfire;

[ShortRunJob]
[MemoryDiagnoser]
public class EnqueueBenchmarks
{
    [GlobalSetup]
    public void Setup()
    {
        GlobalConfiguration.Configuration
            .UseMemoryStorage();
    }

    [Benchmark]
    public string Enqueue()
    {
        return BackgroundJob.Enqueue<NoOpJob>(
            x => x.ExecuteAsync());
    }
}
