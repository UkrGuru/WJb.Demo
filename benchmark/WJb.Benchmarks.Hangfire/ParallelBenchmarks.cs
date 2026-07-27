using BenchmarkDotNet.Attributes;

namespace WJb.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class ParallelBenchmarks
{
    [Params(1000, 10000, 100000)]
    public int Count;

    private IWJb _wjb = null!;

    [GlobalSetup]
    public void Setup()
    {
        _wjb = WJbBuilder.Create(cfg =>
        {
            cfg.AddAction<WJbNoOpAction>();
        });
    }

    [Benchmark]
    public async Task ParallelCreate()
    {
        var tasks = new Task<IAction>[Count];

        for (var i = 0; i < Count; i++)
        {
            tasks[i] = _wjb.CreateAsync<WJbNoOpAction>();
        }

        await Task.WhenAll(tasks);
    }

    [Benchmark]
    public async Task ParallelCreateAndExecute()
    {
        var tasks = new Task[Count];

        for (var i = 0; i < Count; i++)
        {
            tasks[i] = Run();
        }

        await Task.WhenAll(tasks);
    }

    private async Task Run()
    {
        var action = await _wjb.CreateAsync<WJbNoOpAction>();

        await action.ExecuteAsync(null);
    }
}

public sealed class WJbNoOpAction : IAction
{
    public Task<ActionResult> ExecuteAsync(object? input, CancellationToken cancellationToken) 
        => Task.FromResult(ActionResults.None());
}