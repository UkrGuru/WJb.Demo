using BenchmarkDotNet.Attributes;
using WJb.Benchmarks.Infrastructure;

namespace WJb.Benchmarks.Enqueue;

[MemoryDiagnoser]
[ShortRunJob]
public class EnqueueManyBenchmarks
{
    [Params(1_000, 10_000, 100_000)]
    public int Count;

    private IWJb _wjb = null!;

    [GlobalSetup]
    public void Setup()
    {
        _wjb = WJbBuilder.Create(
            new InMemoryStore(),
            cfg => cfg.AddAction<NoOpAction>("noop"));
    }

    [Benchmark]
    public async Task EnqueueMany()
    {
        for (var i = 0; i < Count; i++)
        {
            await _wjb.EnqueueAsync("noop", null);
        }
    }
}