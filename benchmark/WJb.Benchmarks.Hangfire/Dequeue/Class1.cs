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
public class DequeueBenchmarks
{
    private IStorageConnection _connection = null!;

    [GlobalSetup]
    public void Setup()
    {
        GlobalConfiguration.Configuration
            .UseMemoryStorage();

        var jobs = new BackgroundJobClient();

        for (var i = 0; i < 100_000; i++)
        {
            jobs.Enqueue<NoOpJob>(x => x.Execute());
        }

        _connection = JobStorage.Current.GetConnection();
    }

    [Benchmark]
    public IFetchedJob Dequeue()
    {
        return _connection.FetchNextJob(
            ["default"],
            CancellationToken.None);
    }
}