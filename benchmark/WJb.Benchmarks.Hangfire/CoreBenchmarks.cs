using BenchmarkDotNet.Attributes;

namespace WJb.Benchmarks.Hangfire;

[MemoryDiagnoser]
[ShortRunJob]
public class CoreBenchmarks
{
    private LogAction _action = null!;

    [GlobalSetup]
    public Task Setup()
    {
        _action = new LogAction();

        return Task.CompletedTask;
    }

    [Benchmark]
    public Task CreateOnly()
    {
        _ = new LogAction();

        return Task.CompletedTask;
    }

    [Benchmark]
    public async Task ExecuteOnly()
    {
        await _action.ExecuteAsync();
    }

    [Benchmark]
    public async Task CreateAndExecute()
    {
        var action = new LogAction();

        await action.ExecuteAsync();
    }
}

public class LogAction
{
    public Task ExecuteAsync()
    {
        return Task.CompletedTask;
    }
}