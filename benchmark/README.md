# WJb Scheduler Benchmarks

Performance comparison of **WJb**, **Quartz.NET**, and **Hangfire** using identical workloads and validated execution results.

## Benchmark Environment

```text
BenchmarkDotNet v0.15.8
.NET 10.0
Windows 11
Intel Core i7-12700K
20 Logical Cores
```

## Benchmark Methodology

### Enqueue Benchmark

Measures only job submission overhead.

- Workers disabled.
- No job execution during measurement.
- Fresh in-memory storage created for every iteration.
- Validation tests verify that no jobs execute during enqueue-only benchmarks.

### End-to-End Benchmark

Measures the complete processing pipeline:

```text
Enqueue → Schedule → Execute → Complete
```

Configuration:

```text
WJb        : 1 worker
Hangfire   : 1 worker
Quartz.NET : 1 worker
```

All frameworks process the same payload:

```csharp
public sealed record EchoPayload(int Id, string Name);
```

Work performed by each job:

```csharp
Interlocked.Increment(ref Completed);
```

Validation tests verify:

- All jobs execute successfully.
- No jobs are lost.
- Completion counts match expected values.
- Jobs execute exactly once.
- Enqueue benchmarks perform no execution.

---

# Enqueue Performance

## 100 Jobs

| Framework | Time |
|------------|------------:|
| WJb | 330.59 μs |
| Quartz.NET | 482.30 μs |
| Hangfire | 31.53 ms |

## 1,000 Jobs

| Framework | Time |
|------------|------------:|
| WJb | 3.68 ms |
| Quartz.NET | 4.39 ms |
| Hangfire | 233.22 ms |

## 10,000 Jobs

| Framework | Time |
|------------|------------:|
| WJb | 25.54 ms |
| Quartz.NET | 33.88 ms |
| Hangfire | 2,381.42 ms |

## Enqueue Throughput Chart

```text
10,000 Jobs

WJb          25.5 ms  █
Quartz       33.9 ms  █▎
Hangfire   2381.4 ms  ███████████████████████████████████████████████████████████████
```

## Enqueue Memory Usage

### 10,000 Jobs

| Framework | Allocated Memory |
|------------|----------------:|
| WJb | 13.41 MB |
| Quartz.NET | 36.24 MB |
| Hangfire | 213.83 MB |

```text
10,000 Jobs

WJb         13.4 MB  █
Quartz      36.2 MB  ██▋
Hangfire   213.8 MB  ████████████████
```

---

# End-to-End Performance

Measures:

```text
Enqueue → Schedule → Execute → Complete
```

## 100 Jobs

| Framework | Time |
|------------|------------:|
| WJb | 668.6 μs |
| Quartz.NET | 11.24 ms |
| Hangfire | 40.86 ms |
| JobCount | WJb (baseline) | Quartz          | Hangfire              |
|----------|----------------|-----------------|-----------------------|
| 1        | 22.6 µs / 1.3 KB | 1.22× / 3.0×   | **17×** / 17.6×      |
| 100      | 476 µs / 128 KB  | 2.5× / 4.0×    | **73×** / 58.6×      |
| 1000     | 3.58 ms / 1.28 MB| 3.3× / 3.6×    | **85×** / 58.4×      |
| 10000    | 20.5 ms / 12.8 MB| 5.5× / 8.5×    | **128×** / 58.5×     |

## 1,000 Jobs

| Framework | Time |
|------------|------------:|
| WJb | 7.44 ms |
| Quartz.NET | 26.41 ms |
| Hangfire | 259.72 ms |

## 10,000 Jobs

| Framework | Time |
|------------|------------:|
| WJb | 39.94 ms |
| Quartz.NET | 89.09 ms |
| Hangfire | 2,598.00 ms |

## End-to-End Throughput Chart

```text
10,000 Jobs

WJb          39.9 ms  █
Quartz       89.1 ms  ██▎
Hangfire   2598.0 ms  ███████████████████████████████████████████████████████████████
```

## End-to-End Memory Usage

### 10,000 Jobs

| Framework | Allocated Memory |
|------------|----------------:|
| WJb | 23.92 MB |
| Quartz.NET | 70.92 MB |
| Hangfire | 714.67 MB |

```text
10,000 Jobs

WJb         23.9 MB  █
Quartz      70.9 MB  ██▉
Hangfire   714.7 MB  ██████████████████████████████
```

---

# Summary

### Enqueue (10,000 Jobs)

- **WJb is ~1.3× faster than Quartz.NET**
- **WJb is ~93× faster than Hangfire**
- **WJb uses ~2.7× less memory than Quartz.NET**
- **WJb uses ~16× less memory than Hangfire**

### End-to-End (10,000 Jobs)

- **WJb is ~2.2× faster than Quartz.NET**
- **WJb is ~65× faster than Hangfire**
- **WJb uses ~3× less memory than Quartz.NET**
- **WJb uses ~30× less memory than Hangfire**

## Notes

- Benchmarks use in-memory storage only.
- Startup and teardown costs are excluded from measurements.
- Concurrency is normalized to a single worker for all frameworks.
- Results represent framework overhead for lightweight jobs and should not be interpreted as a universal performance ranking for all production workloads.
