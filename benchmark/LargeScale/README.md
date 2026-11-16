# WJb Performance Evolution

Benchmark: 1,000,000 jobs, single worker, in-memory storage

| Stage                               | Jobs/sec | Gain vs Baseline |
|-------------------------------------|---------:|-----------------:|
| Baseline (.NET 10)                  | 181,697  | -                |
| .NET 11 + InMemoryStore + StoreBase | 206,070  | +13.4%           |
| JobExecute Update                   | 211,188  | +16.2%           |
| WasmWorker Update                   | 213,723  | +17.6%           |

## Current WJb Result

Completed   : 1,000,000
Expected    : 1,000,000
Sum         : 499,999,500,000
ExpectedSum : 499,999,500,000
Verification: PASSED

Elapsed     : 00:00:04.6789558
Jobs/sec    : 213,723
Memory      : 993.87 MB

## Competitors

### Quartz

Completed   : 1,000,000
Expected    : 1,000,000
Sum         : 499,999,500,000
ExpectedSum : 499,999,500,000
Verification: PASSED

Elapsed     : 00:00:24.8474230
Jobs/sec    : 40,246
Memory      : 1,492.51 MB

### Hangfire

Completed   : 1,000,000
Expected    : 1,000,000
Sum         : 499,999,500,000
ExpectedSum : 499,999,500,000
Verification: PASSED

Elapsed     : 00:05:55.4745246
Jobs/sec    : 2,813
Memory      : 2,527.67 MB

## Comparison

WJb vs Quartz   : 5.31x faster
WJb vs Hangfire : 75.98x faster

Memory Usage

WJb      :   993.87 MB
Quartz   : 1,492.51 MB
Hangfire : 2,527.67 MB

## Performance Chart

Jobs/sec

220k |                                        ● WJb (213,723)
200k |
180k |
160k |
140k |
120k |
100k |
 80k |
 60k |
 40k |         ● Quartz (40,246)
 20k |
   0 | ● Hangfire (2,813)
      +------------------------------------------------

## Notes

WJb processed 1,000,000 jobs in under 5 seconds.

Quartz completed the same workload in about 25 seconds.

Hangfire completed the same workload in nearly 6 minutes.

By the time Hangfire finished the benchmark, my coffee was already cold. ☕😄

Apparently, Hangfire treats one million jobs as a serious enterprise process.