# WJb Scheduler Benchmarks

Performance comparison of **WJb**, **Quartz.NET**, and **Hangfire** for job enqueue operations.

## Benchmark Environment

```text
BenchmarkDotNet v0.15.8
.NET 10.0
Windows 11
Intel Core i7-12700K
20 Logical Cores
```

## Scenario

Each framework enqueues the same payload:

```csharp
public sealed record EchoPayload(int Id, string Name);
```

Validation tests are executed before benchmarking to verify that all frameworks successfully process the same workload and produce identical completion counts.

## Job Enqueue Performance

| JobCount | WJb (baseline) | Quartz          | Hangfire              |
|----------|----------------|-----------------|-----------------------|
| 1        | 22.6 µs / 1.3 KB | 1.22× / 3.0×   | **17×** / 17.6×      |
| 100      | 476 µs / 128 KB  | 2.5× / 4.0×    | **73×** / 58.6×      |
| 1000     | 3.58 ms / 1.28 MB| 3.3× / 3.6×    | **85×** / 58.4×      |
| 10000    | 20.5 ms / 12.8 MB| 5.5× / 8.5×    | **128×** / 58.5×     |

## Enqueue Throughput Chart

```text
10,000 Jobs

WJb         20 ms  █
Quartz     113 ms  █████▌
Hangfire 2,613 ms  ████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
```

## Memory Consumption

### 10,000 Jobs

| Framework | Allocated Memory |
|------------|----------------:|
| WJb | 12.81 MB |
| Quartz.NET | 108.89 MB |
| Hangfire | 749.68 MB |

## Memory Usage Chart

```text
10,000 Jobs

WJb        12.8 MB  █
Quartz    108.9 MB  ████████▌
Hangfire  749.7 MB  ███████████████████████████████████████████████████████████
```

## Conclusion

For high-throughput job scheduling workloads, **WJb** demonstrates:

- **5.5× higher enqueue throughput** than Quartz.NET.
- **128× higher enqueue throughput** than Hangfire.
- **8.5× lower memory consumption** than Quartz.NET.
- **58× lower memory consumption** than Hangfire.
