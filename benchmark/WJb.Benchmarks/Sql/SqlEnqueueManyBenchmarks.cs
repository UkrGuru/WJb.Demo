using BenchmarkDotNet.Attributes;
using Microsoft.Data.SqlClient;
using WJb.Sql;

namespace WJb.Benchmarks.Sql;

[ShortRunJob]
[MemoryDiagnoser]
public class SqlEnqueueManyBenchmarks
{
    private const string DbName = "WJb0622";

    private const string MasterConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Initial Catalog=master;";

    private readonly string _connectionString;

    private readonly SqlStore _store;

    public SqlEnqueueManyBenchmarks()
    {
        _connectionString = $"{MasterConnectionString};Database={DbName};";

        _store = new SqlStore(() => new SqlConnection(_connectionString));
    }

    [Params(1000, 10000, 100000)]
    public int Count;

    [IterationSetup]
    public void Setup()
    {
        using var conn = new SqlConnection(_connectionString);

        conn.ExecAsync(
            """
            TRUNCATE TABLE WJb_Jobs;
            """
        )
        .GetAwaiter()
        .GetResult();
    }

    [Benchmark(Baseline = true)]
    public async Task EnqueueManySequential()
    {
        for (var i = 0; i < Count; i++)
            await _store.EnqueueAsync("noop", null);
    }

    [Benchmark]
    public async Task EnqueueMany()
    {
        async IAsyncEnumerable<object?> Payloads()
        {
            for (var i = 0; i < Count; i++)
                yield return null;
        }

        await _store.EnqueueManyAsync("noop", Payloads());
    }
}