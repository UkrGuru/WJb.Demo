// Benchmark scenario drafted with AI assistance and reviewed by the WJb author.
// Validate results independently before using them for performance claims.

using BenchmarkDotNet.Attributes;
using WJb.Benchmarks.Infrastructure;

namespace WJb.Benchmarks.Core;

[MemoryDiagnoser]
[ShortRunJob]
public class StartupBenchmarks
{
    [Benchmark]
    public IActionFactory Create()
    {
        return WJbBuilder.Create(cfg =>
        {
            cfg.AddAction<NoOpAction>();
        });
    }
}