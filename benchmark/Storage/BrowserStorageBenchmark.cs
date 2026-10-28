using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.JSInterop;
using WJb;
using WJb.IndexedDB;

namespace Benchmarks;

[SimpleJob(RuntimeMoniker.Net10_0)]
[MemoryDiagnoser]
public class BrowserStorageBenchmark
{
    [Params(1, 10, 100, 1000)]
    public int JobCount { get; set; }

    private readonly FakeJsRuntime _js = new();

    [IterationSetup]
    public void IterationSetup()
    {
        _js.Clear();
    }

    [Benchmark(Baseline = true)]
    public Task InMemory()
        => RunAsync(new InMemoryStore());

    [Benchmark]
    public Task IndexedDb()
        => RunAsync(new IdbStore(_js));

    private async Task RunAsync(IStore store)
    {
        for (int i = 0; i < JobCount; i++)
        {
            await store.EnqueueAsync(
                "echo",
                new EchoPayload(i, $"User {i}"));
        }

        var processed = 0;

        while (await store.DequeueAsync() is not null)
        {
            processed++;
        }

        if (processed != JobCount)
        {
            throw new InvalidOperationException(
                $"Expected={JobCount}, Actual={processed}");
        }
    }

    private sealed class FakeJsRuntime : IJSRuntime
    {
        private readonly Dictionary<string, object?> _data = [];

        public void Clear()
        {
            _data.Clear();
        }

        public ValueTask<TValue> InvokeAsync<TValue>(
            string identifier,
            object?[]? args)
        {
            return InvokeAsync<TValue>(
                identifier,
                CancellationToken.None,
                args);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(
            string identifier,
            CancellationToken cancellationToken,
            object?[]? args)
        {
            object? result = null;

            switch (identifier)
            {
                case "wjbIdb.set":
                    {
                        var store = (string)args![0]!;
                        var key = (string)args[1]!;

                        _data[$"{store}:{key}"] = args[2];
                        break;
                    }

                case "wjbIdb.get":
                    {
                        var store = (string)args![0]!;
                        var key = (string)args[1]!;

                        _data.TryGetValue(
                            $"{store}:{key}",
                            out result);

                        break;
                    }

                case "wjbIdb.remove":
                    {
                        var store = (string)args![0]!;
                        var key = (string)args[1]!;

                        result = _data.Remove(
                            $"{store}:{key}");

                        break;
                    }

                case "wjbIdb.exists":
                    {
                        var store = (string)args![0]!;
                        var key = (string)args[1]!;

                        result = _data.ContainsKey(
                            $"{store}:{key}");

                        break;
                    }

                case "wjbIdb.getAll":
                    {
                        var store = (string)args![0]!;

                        if (typeof(TValue) == typeof(List<JobInfo>))
                        {
                            var list = _data
                                .Where(x => x.Key.StartsWith(
                                    store + ":",
                                    StringComparison.Ordinal))
                                .Select(x => (JobInfo)x.Value!)
                                .ToList();

                            return ValueTask.FromResult(
                                (TValue)(object)list);
                        }

                        return ValueTask.FromResult(
                            (TValue)(object)new List<JobInfo>());
                    }

                case "wjbIdb.keys":
                    {
                        var store = (string)args![0]!;

                        result = _data.Keys
                            .Where(x => x.StartsWith(
                                store + ":",
                                StringComparison.Ordinal))
                            .Select(x => x[(store.Length + 1)..])
                            .ToList();

                        break;
                    }
            }

            return ValueTask.FromResult(
                (TValue?)result!);
        }
    }
}
