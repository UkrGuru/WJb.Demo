using BenchmarkDotNet.Attributes;
using WJb;

namespace WJb.Benchmarks.Dequeue;

[MemoryDiagnoser]
[ShortRunJob]
public class DequeueBenchmarks
{
    private InMemoryStore _store = null!;

    [GlobalSetup]
    public async Task Setup()
    {
        _store = new InMemoryStore();

        for (var i = 0; i < 100_000; i++)
        {
            await _store.EnqueueAsync("noop");
        }
    }

    [Benchmark]
    public Task<JobEnvelope?> Dequeue()
        => _store.DequeueAsync();
}