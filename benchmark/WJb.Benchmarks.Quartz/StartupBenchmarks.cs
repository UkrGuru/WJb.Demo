using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using Quartz;

[MemoryDiagnoser]
[ShortRunJob]
public class StartupBenchmarks
{
    [Benchmark]
    public ServiceProvider WJb()
    {
        var services = new ServiceCollection();

        //services.AddWJb();

        return services.BuildServiceProvider();
    }

    [Benchmark]
    public ServiceProvider Hangfire()
    {
        var services = new ServiceCollection();

        services.AddHangfire(_ => { });

        return services.BuildServiceProvider();
    }

    [Benchmark]
    public ServiceProvider Quartz()
    {
        var services = new ServiceCollection();

        services.AddQuartz();

        return services.BuildServiceProvider();
    }
}