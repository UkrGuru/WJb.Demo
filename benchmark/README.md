# WJb vs Hangfire vs Quartz: I Finally Measured Them

For months I kept seeing the same advice:

> Just use Hangfire.
>
> Just use Quartz.

Fair enough.

Both projects are mature, widely used, and have proven themselves in production.

But I wanted actual numbers.

So I built a BenchmarkDotNet suite and compared:

- WJb
- Hangfire
- Quartz

The source code is public:

https://github.com/UkrGuru/WJb.Demo/tree/main/benchmark

![WJb Benchmarks](https://github.com/UkrGuru/WJb.Demo/raw/main/assets/wJb-benchmarks.gif)

---

## Why I Built WJb

WJb started as a simple idea:

```text
enqueue
  ↓
job
  ↓
action
```

No visual workflow designer.

No dashboard dependency.

No hidden runtime magic.

Just a small background job engine focused on execution speed, predictable behavior, and simple composition.

Because of that design goal, performance has always been important.

So I wanted to compare the actual overhead.

---

## Benchmark Environment

- .NET 10
- BenchmarkDotNet
- Release build
- In-memory configuration
- Same machine
- Same benchmark suite

The purpose was not to declare a winner for every scenario.

The purpose was to measure queueing and execution overhead using comparable workloads.

---

## Test #1: Single Enqueue

How fast can the library accept one job?

| Library | Time | Memory |
|----------|----------:|----------:|
| WJb | 349 ns | 328 B |
| Quartz | 3.848 μs | 3.08 KB |
| Hangfire | 6.212 μs | 11.46 KB |

### Result

WJb was:

- ~11× faster than Quartz
- ~18× faster than Hangfire

For a single operation this difference may look small.

At scale it becomes much more noticeable.

---

## Test #2: Enqueue 100,000 Jobs

A more realistic stress test.

| Library | Time | Memory |
|----------|----------:|----------:|
| WJb | 32 ms | 31 MB |
| Hangfire | 743 ms | 1063 MB |
| Quartz | 902 ms | 539 MB |

### Result

WJb completed the same workload:

- ~23× faster than Hangfire
- ~28× faster than Quartz

Memory consumption was also significantly lower.

---

## Test #3: Parallel Producers

100,000 jobs created by multiple parallel producers.

Best observed result:

| Library | Time |
|----------|----------:|
| WJb | 52 ms |
| Hangfire | 540 ms |
| Quartz | 600 ms |

This scenario simulates bursts of activity from multiple application threads.

---

## Test #4: Queue Throughput

After implementing dedicated dequeue benchmarks, I measured queue consumption performance.

### Single Dequeue

| Operation | Result |
|----------|----------:|
| WJb Dequeue | ~15 ns |

### 100,000 Dequeues

| Operation | Result |
|----------|----------:|
| WJb DequeueMany | ~29 ms |

### Full Queue Lifecycle

Enqueue followed by dequeue.

| Jobs | Time | Memory |
|----------:|----------:|----------:|
| 1,000 | 2.96 ms | 1.11 MB |
| 10,000 | 33.35 ms | 11.51 MB |
| 100,000 | 167.83 ms | 118.99 MB |

For the largest test this corresponds to roughly:

```text
~600,000 jobs/sec
```

through the public API.

---

## What These Results Mean

They do not mean:

- Hangfire is bad
- Quartz is bad
- Every workload should use WJb

What they do show is that reducing abstraction layers and keeping the core execution model small can dramatically reduce overhead.

The benchmark results suggest that simplicity pays off.

---

## Feature Trade-Offs

Performance is only one dimension.

Hangfire and Quartz provide a broader ecosystem and solve additional problems.

Depending on requirements, those capabilities may be more important than raw throughput.

Choose the tool that matches your actual problem.

---

## Reproduce Everything

No screenshots.

No special hardware claims.

No hidden setup.

Everything is available in the repository:

https://github.com/UkrGuru/WJb.Demo/tree/main/benchmark

Run the benchmarks yourself:

```bash
dotnet run -c Release
```

---

## Repository

- Demo: https://github.com/UkrGuru/WJb.Demo
- Benchmarks: https://github.com/UkrGuru/WJb.Demo/tree/main/benchmark
- QuickStart: https://github.com/UkrGuru/WJb.Demo/tree/main/quickstart/WJb.Demo.QuickStart

---

## Question

If you're using Hangfire or Quartz today:

**What feature would make you choose a slower background job library?**

I'm genuinely curious.
