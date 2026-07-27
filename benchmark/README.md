# WJb vs Hangfire vs Quartz: I Finally Measured Them

For months I kept seeing the same advice:

> Just use Hangfire.
>
> Just use Quartz.

Fair enough.

But I wanted actual numbers.

So I built a small BenchmarkDotNet suite and compared:

- WJb
- Hangfire
- Quartz

using equivalent enqueue scenarios.

The source code is public:

https://github.com/UkrGuru/WJb.Demo/tree/main/benchmark

![WJb Benchmarks](https://github.com/UkrGuru/WJb.Demo/raw/main/assets/wJb-benchmarks.gif)

---

## Test #1: Single Enqueue

How fast can the library accept one job?

| Library | Time | Memory |
|----------|----------:|----------:|
| WJb | 349 ns | 328 B |
| Quartz | 3.848 μs | 3.08 KB |
| Hangfire | 6.212 μs | 11.46 KB |

WJb was:

- ~11× faster than Quartz
- ~18× faster than Hangfire

---

## Test #2: 100,000 Jobs

A more realistic stress test.

| Library | Time | Memory |
|----------|----------:|----------:|
| WJb | 32 ms | 31 MB |
| Hangfire | 743 ms | 1063 MB |
| Quartz | 902 ms | 539 MB |

WJb completed the same workload:

- ~23× faster than Hangfire
- ~28× faster than Quartz

---

## Test #3: Parallel Producers

100,000 jobs from multiple producers.

Best observed result:

| Library | Time |
|----------|----------:|
| WJb | 52 ms |
| Hangfire | 540 ms |
| Quartz | 600 ms |

---

## The Interesting Part

The goal of WJb was never:

- dashboards
- workflow designers
- visual editors
- hidden pipelines

The goal was always:

```text
enqueue
  ↓
job
  ↓
action
```

Nothing else.

The benchmark results suggest that the simplicity pays off.

---

## Reproduce Everything

No screenshots.

No marketing claims.

No hidden setup.

Everything is in the repository:

https://github.com/UkrGuru/WJb.Demo/tree/main/benchmark

Run it yourself:

```bash
dotnet run -c Release
```

---

## Question

If you're using Hangfire or Quartz today:

What feature would make you choose a slower background job library?

I'm genuinely curious.