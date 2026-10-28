using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using WJb;
using WJb.Extensions;
using WJb.Sqlite;
using WJb.SqlServer;

namespace Benchmarks;

[SimpleJob(RuntimeMoniker.Net10_0)]
[MemoryDiagnoser]
public class PersistentStorageBenchmark
{
    [Params(1, 10, 100)]  // , 1000, 10000
    public int JobCount { get; set; }

    private const string SqliteConnectionString =
        "Data Source=wjb-bench.db";

    private const string SqlServerConnectionString =
        "Server=(localdb)\\MSSQLLocalDB2025;Database=WJbBench;Trusted_Connection=True;TrustServerCertificate=True";

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        await WJbSqlite.InitDbAsync(SqliteConnectionString);
        await WJbSqlServer.InitDbAsync(SqlServerConnectionString);

        await using var cn1 = new SqlConnection(SqlServerConnectionString);
        await cn1.OpenAsync();
        await cn1.ExecAsync("TRUNCATE TABLE dbo.WJb_Jobs");

        await using var cn2 = new SqliteConnection(SqliteConnectionString);
        await cn2.OpenAsync();
        await cn2.ExecAsync("DELETE FROM WJb_Jobs");
    }

    [Benchmark(Baseline = true)]
    public async Task SQLite()
    {
        var store = new SqliteStore(
            () => new SqliteConnection(SqliteConnectionString));

        await RunAsync(store);
    }

    [Benchmark]
    public async Task SqlServer()
    {
        var store = new MsSqlStore(
            () => new SqlConnection(SqlServerConnectionString));

        await RunAsync(store);
    }

    private async Task RunAsync(IStore store)
    {
        for (int i = 0; i < JobCount; i++)
        {
            await store.EnqueueAsync(
                "echo",
                new EchoPayload(i, $"User {i}"));
        }

        var processed = 0;

        while (true)
        {
            var job = await store.DequeueAsync();

            if (job is null)
                break;

            processed++;
        }

        if (processed != JobCount)
        {
            throw new InvalidOperationException(
                $"Expected={JobCount}, Actual={processed}");
        }
    }
}

public sealed record EchoPayload(int Id, string Name);