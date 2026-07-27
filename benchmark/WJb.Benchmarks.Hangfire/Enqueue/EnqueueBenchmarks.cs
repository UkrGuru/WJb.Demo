// Benchmark scenario drafted with AI assistance and reviewed by the WJb author.
// Validate results independently before using them for performance claims.

using BenchmarkDotNet.Attributes;
using Hangfire;
using Hangfire.MemoryStorage;
using WJb.Benchmarks.Infrastructure;

namespace WJb.Benchmarks.Enqueue;

[MemoryDiagnoser]
[ShortRunJob]
public class EnqueueBenchmarks
{
    private BackgroundJobClient _jobs = null!;

    [GlobalSetup]
    public void Setup()
    {
        GlobalConfiguration.Configuration
            .UseMemoryStorage();

        _jobs = new BackgroundJobClient();
    }

    [Benchmark]
    public string Enqueue()
    {
        return _jobs.Enqueue<NoOpJob>(x => x.Execute());
    }
}

