# ⚡ WJb Benchmark

End-to-end benchmark comparing WJb, Hangfire, and Quartz.NET using 10,000 in-memory echo jobs.

## Results

```text
| Framework | Time      | Memory |
|------------|-----------|--------|
| WJb        |   43.17 ms| 27.59 MB |
| Quartz.NET |   52.07 ms| 46.55 MB |
| Hangfire   | 2929.46 ms| 772.17 MB |
```

## Throughput

```text
WJb       ≈ 231,642 jobs/sec
Quartz    ≈ 192,049 jobs/sec
Hangfire  ≈   3,414 jobs/sec
```

## Test

```text
Enqueue
 ↓
Execute
 ↓
Complete
```

Hardware:

```text
Intel Core i7-12700K
.NET 10
Windows 11
BenchmarkDotNet 0.15.8
```

## WJb Action

```csharp
[ActionName("echo")]
public sealed class WJbEchoAction : JobAction<EchoPayload?>
{
    public override ValueTask<ActionResult> ExecuteAsync(
        EchoPayload? payload,
        CancellationToken ct = default)
        => ValueTask.FromResult(Results.Done(payload));
}
```

## Summary

- Fastest execution time
- Lowest memory allocation
- Explicit workflow model
- Minimal runtime overhead

> Results reflect this specific benchmark configuration and workload.
