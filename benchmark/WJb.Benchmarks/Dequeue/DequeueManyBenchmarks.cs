using BenchmarkDotNet.Attributes;

namespace WJb.Benchmarks.Dequeue;

[MemoryDiagnoser]
[ShortRunJob]
public class DequeueManyBenchmarks
{
    [Params(1_000, 10_000, 100_000)]
    public int Count;

    private InMemoryStore _store = null!;

    [IterationSetup]
    public void Setup()
    {
        _store = new InMemoryStore();

        for (var i = 0; i < Count; i++)
        {
            _store.EnqueueAsync("noop")
                .GetAwaiter()
                .GetResult();
        }
    }

    [Benchmark]
    public async Task DequeueMany()
    {
        for (var i = 0; i < Count; i++)
        {
            await _store.DequeueAsync();
        }
    }
}