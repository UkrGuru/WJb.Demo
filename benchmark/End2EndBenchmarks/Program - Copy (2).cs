using System.Threading.Channels;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using WJb;

BenchmarkRunner.Run<End2EndTest>();

[ShortRunJob]
[MemoryDiagnoser]
public class End2EndTest
{
    [Params(1_000)]
    public int Preloaded { get; set; }

    [Params(100)]
    public int ProcessCount { get; set; }

    private InMemoryStore _store = null!;
    private IWJb _wjb = null!;

    private Channel<EchoPayload> _channel = null!;

    [GlobalSetup]
    public void Setup()
    {
        _store = new InMemoryStore();
        _wjb = WJbBuilder.Create(_store, cfg =>
        {
            cfg.AddAction<WJbEchoAction>("echo");
        });

        _channel = Channel.CreateUnbounded<EchoPayload>(new UnboundedChannelOptions
        {
            SingleWriter = true,
            SingleReader = true
        });

        for (int i = 0; i < Preloaded; i++)
        {
            var payload = new EchoPayload(i, $"Pre {i}");

            _wjb.EnqueueAsync("echo", payload).GetAwaiter().GetResult();

            var result = _channel.Writer.TryWrite(payload);
        }
    }

    [Benchmark(Baseline = true)]
    public async Task WJb_End2End_Echo()
    {
        for (int i = 0; i < ProcessCount; i++)
        {
            await _wjb.EnqueueAsync("echo", new EchoPayload(i, $"User {i}"));

            await _wjb.ExecuteOnceAsync();
        }
    }

    [Benchmark]
    public async Task Channels_End2End_Echo()
    {
        for (int i = 0; i < ProcessCount; i++)
        {
            _channel.Writer.TryWrite(new EchoPayload(i, $"User {i}"));

            if (await _channel.Reader.WaitToReadAsync())
            {
                _channel.Reader.TryRead(out var payload);

                _ = payload;
            }
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _wjb.ExecuteLoopAsync().GetAwaiter().GetResult();
        _channel.Writer.Complete();
    }
}
