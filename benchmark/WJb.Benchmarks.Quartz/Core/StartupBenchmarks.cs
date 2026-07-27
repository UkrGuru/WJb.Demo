// Benchmark scenario drafted with AI assistance and reviewed by the WJb author.
// Validate results independently before using them for performance claims.

using BenchmarkDotNet.Attributes;
using Quartz.Impl;

namespace WJb.Benchmarks.Core;

[MemoryDiagnoser]
[ShortRunJob]
public class StartupBenchmarks
{
    [Benchmark]
    public StdSchedulerFactory Create()
    {
        return new StdSchedulerFactory();
    }
}