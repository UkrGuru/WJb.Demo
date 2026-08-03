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
public class DequeueManyBenchmarks
{
    [Params(1_000, 10_000, 100_000)]
    public int Count;

    private IStorageConnection _connection = null!;

    [IterationSetup]
    public void Setup()
    {
        GlobalConfiguration.Configuration
            .UseMemoryStorage();

        var jobs = new BackgroundJobClient();

        for (var i = 0; i < Count; i++)
        {
            jobs.Enqueue<NoOpJob>(x => x.Execute());
        }

        _connection = JobStorage.Current.GetConnection();
    }

    [Benchmark]
    public void DequeueMany()
    {
        var fetched = 0;

        for (; ; )
        {
            try
            {
                using var job =
                    _connection.FetchNextJob(
                        ["default"],
                        CancellationToken.None);

                job.RemoveFromQueue();

                fetched++;
            }
            catch
            {
                break;
            }
        }
    }
}