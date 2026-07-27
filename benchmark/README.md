# WJb Benchmarks

BenchmarkDotNet benchmarks comparing:

- WJb
- Hangfire
- Quartz

![WJb Benchmarks](https://github.com/UkrGuru/WJb.Demo/blob/main/assets/wJb-benchmarks.gif)

## Repositories

```text
WJb.Benchmarks
WJb.Benchmarks.Hangfire
WJb.Benchmarks.Quartz
```

## Results

### Enqueue

| Library | Time | Memory |
|----------|----------:|----------:|
| WJb | 349 ns | 328 B |
| Quartz | 3.848 μs | 3.08 KB |
| Hangfire | 6.212 μs | 11.46 KB |

### EnqueueMany (100,000 jobs)

| Library | Time | Memory |
|----------|----------:|----------:|
| WJb | 32 ms | 31 MB |
| Hangfire | 743 ms | 1063 MB |
| Quartz | 902 ms | 539 MB |

### ParallelEnqueue (100,000 jobs)

| Library | Best Time |
|----------|----------:|
| WJb | 52 ms |
| Hangfire | 540 ms |
| Quartz | 600 ms |

## Run

```bash
dotnet run -c Release
```

## Notes

- BenchmarkDotNet ShortRun.
- In-memory storage implementations.
- No-op jobs/actions.
- Results are environment-specific.
- Full source code is included.

## Status

Current benchmark suite:

- Enqueue
- EnqueueMany
- ParallelEnqueue

New benchmarks will be added only when equivalent scenarios can be implemented for all compared libraries.
