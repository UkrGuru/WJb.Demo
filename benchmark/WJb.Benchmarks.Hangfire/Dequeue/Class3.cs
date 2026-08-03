// Benchmark scenario drafted with AI assistance and reviewed by the WJb author.
// Validate results independently before using them for performance claims.

using BenchmarkDotNet.Attributes;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.Storage;
using WJb.Benchmarks.Infrastructure;

namespace WJb.Benchmarks.Dequeue;

[MemoryDiagnoser]
[ShortRunJob]
public class QueueRoundTripBenchmarks
{
    [Params(1_000, 10_000, 100_000)]
    public int Count;

    private BackgroundJobClient _jobs = null!;
    private IStorageConnection _connection = null!;

    [IterationSetup]
    public void Setup()
    {
        GlobalConfiguration.Configuration
            .UseMemoryStorage();

        _jobs = new BackgroundJobClient();
        _connection = JobStorage.Current.GetConnection();
    }

    [Benchmark]
    public void EnqueueAndDequeue()
    {
        for (var i = 0; i < Count; i++)
        {
            _jobs.Enqueue<NoOpJob>(x => x.Execute());
        }

        for (var i = 0; i < Count; i++)
        {
            _connection.FetchNextJob(
                ["default"],
                CancellationToken.None);
        }
    }
}