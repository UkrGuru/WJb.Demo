using BenchmarkDotNet.Attributes;

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
    public Task CreateOnly() => _wjb.CreateAsync<NoOpAction>();

    [Benchmark]
    public Task<IActionResult> ExecuteOnly() => _action.ExecuteAsync(null);

    [Benchmark]
    public async Task<IActionResult> CreateAndExecute()
        => await (await _wjb.CreateAsync<NoOpAction>()).ExecuteAsync(null);
}