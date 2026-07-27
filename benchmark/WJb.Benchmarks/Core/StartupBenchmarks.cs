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