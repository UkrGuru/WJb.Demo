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

### 1 Job

| Framework | Time |
|------------|------------:|
| WJb | 22.62 μs |
| Quartz.NET | 27.50 μs |
| Hangfire | 389.62 μs |

### 100 Jobs

| Framework | Time |
|------------|------------:|
| WJb | 475.52 μs |
| Quartz.NET | 1.18 ms |
| Hangfire | 34.18 ms |

### 1,000 Jobs

| Framework | Time |
|------------|------------:|
| WJb | 3.58 ms |
| Quartz.NET | 9.98 ms |
| Hangfire | 254.84 ms |

### 10,000 Jobs

| Framework | Time |
|------------|------------:|
| WJb | 20.46 ms |
| Quartz.NET | 113.01 ms |
| Hangfire | 2,612.89 ms |

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
