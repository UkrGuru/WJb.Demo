using BenchmarkDotNet.Attributes;
using WJb;
using WJb.Benchmarks;

[ShortRunJob]
[MemoryDiagnoser]
public class ScaleBenchmarks
{
    [Params(1, 10, 100, 1000)]
    public int Actions;

    private IWJb _wjb = null!;

    [GlobalSetup]
    public void Setup()
    {
        _wjb = WJbBuilder.Create(cfg =>
        {
            for (var i = 0; i < Actions; i++)
            {
                cfg.AddAction<LogAction>();
            }
        });
    }

    [Benchmark]
    public async Task CreateAction()
    {
        await _wjb.CreateAsync<LogAction>();
    }
}