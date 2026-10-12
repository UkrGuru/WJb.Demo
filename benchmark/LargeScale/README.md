# Large Scale End-to-End Benchmark

Real-world processing benchmark using:

```text
1,000,000 jobs
1 worker/thread
In-memory storage
```

## Benchmark Environment

```text
.NET 10.0
Windows 11
Intel Core i7-12700K
```

## Workload

```csharp
public sealed record EchoPayload(int Id, string Name);
```

Job implementation:

```csharp
Interlocked.Increment(ref Completed);
```

## Results

| Framework | Time | Jobs/sec | Memory |
|-----------|-----------:|-----------:|-----------:|
| WJb | 4.80 sec | 208,344 | 983.72 MB |
| Quartz.NET | 23.84 sec | 41,940 | 1,292.79 MB |
| Hangfire | 299.00 sec | 3,344 | 2,518.56 MB |

## Throughput

```text
WJb       208,344 jobs/sec
Quartz     41,940 jobs/sec
Hangfire    3,344 jobs/sec
```

## Processing Time

```text
1,000,000 Jobs

WJb          4.80 sec
Quartz      23.84 sec
Hangfire   299.00 sec
```

## Summary

- WJb processed 1,000,000 jobs approximately 5× faster than Quartz.NET.
- WJb processed 1,000,000 jobs approximately 62× faster than Hangfire.
- WJb used approximately 31% less memory than Quartz.NET.
- WJb used approximately 61% less memory than Hangfire.

## Notes

- In-memory storage only.
- Single worker/thread for all frameworks.
- End-to-end execution includes enqueue, scheduling, execution, and completion.
- Results were measured using standalone console applications.
