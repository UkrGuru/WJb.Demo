using BenchmarkDotNet.Attributes;
using WJb.Benchmarks.Infrastructure;

namespace WJb.Benchmarks.Enqueue;

[MemoryDiagnoser]
[ShortRunJob]
public class EnqueueBenchmarks
{
    private IWJb _wjb = null!;

    [GlobalSetup]
    public void Setup()
    {
        _wjb = WJbBuilder.Create(
            new InMemoryStore(),
            cfg => cfg.AddAction<NoOpAction>("noop"));
    }

    [Benchmark]
    public Task Enqueue()
        => _wjb.EnqueueAsync("noop", null);
}
