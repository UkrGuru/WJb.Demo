using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WJb.Benchmarks.Dequeue;

[MemoryDiagnoser]
[ShortRunJob]
public class QueueRoundTripBenchmarks
{
    [Params(1_000, 10_000, 100_000)]
    public int Count;

    private InMemoryStore _store = null!;

    [IterationSetup]
    public void Setup()
    {
        _store = new InMemoryStore();
    }

    [Benchmark]
    public async Task EnqueueAndDequeue()
    {
        for (var i = 0; i < Count; i++)
        {
            await _store.EnqueueAsync("noop");
        }

        for (var i = 0; i < Count; i++)
        {
            await _store.DequeueAsync();
        }
    }
}