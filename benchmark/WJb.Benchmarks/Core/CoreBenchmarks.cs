using BenchmarkDotNet.Attributes;
using WJb.Benchmarks.Infrastructure;

namespace WJb.Benchmarks.Core;

[MemoryDiagnoser]
[ShortRunJob]
public class CoreBenchmarks
{
    private IActionFactory _wjb = null!;
    private IAction _action = null!;

    [GlobalSetup]
    public async Task Setup()
    {
        _wjb = WJbBuilder.Create(cfg =>
        {
            cfg.AddAction<NoOpAction>();
        });

        _action = await _wjb.CreateAsync<NoOpAction>();
    }

    [Benchmark]
    public Task CreateOnly()
    {
        return _wjb.CreateAsync<NoOpAction>();
    }

    [Benchmark]
    public Task<ActionResult> ExecuteOnly()
    {
        return _action.ExecuteAsync(null);
    }

    [Benchmark]
    public async Task<ActionResult> CreateAndExecute()
    {
        var action = await _wjb.CreateAsync<NoOpAction>();

        return await action.ExecuteAsync(null);
    }
}