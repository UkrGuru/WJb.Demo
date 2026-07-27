using BenchmarkDotNet.Attributes;
using Hangfire;
using Hangfire.MemoryStorage;

namespace WJb.Benchmarks.Core;

[MemoryDiagnoser]
[ShortRunJob]
public class StartupBenchmarks
{
    [Benchmark]
    public IGlobalConfiguration Create()
    {
        return GlobalConfiguration.Configuration
            .UseMemoryStorage();
    }
}