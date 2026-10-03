# WJb.StressTests

Stress and reliability test suite for **WJb**.

This project validates queue correctness, concurrency safety, memory stability, and reliability under sustained load.

## Status

![Stress Tests](https://raw.githubusercontent.com/UkrGuru/WJb.Demo/main/assets/stress-tests.png)

## Highlights

### Sustained Load
```
Produced   : 62,466,490
Consumed   : 62,466,490
Difference : 0
```
A two-minute producer/consumer run processed more than **62 million jobs** without losing a single job.

### High Load Producer / Consumer
```
Produced   : 2,000,000
Consumed   : 2,000,000
Time       : 00:00:04.2282307
Throughput : 473,011 jobs/sec
```
Concurrent producers and consumers maintained perfect consistency under heavy load.

### Fully Concurrent 5M Jobs
```
5M jobs processed in 00:00:17.0693243
```
Five million jobs were successfully enqueued and processed under fully concurrent execution.

### Memory Stability
```
Cycle 1: 390,464,888 bytes | ×1.00 base | ×1.00 prev
Cycle 2: 391,960,880 bytes | ×1.00 base | ×1.00 prev
Cycle 3: 393,943,080 bytes | ×1.01 base | ×1.01 prev
Cycle 4: 393,986,248 bytes | ×1.01 base | ×1.00 prev
Cycle 5: 394,078,152 bytes | ×1.01 base | ×1.00 prev
```
Memory remained effectively flat across repeated fill/drain cycles, indicating stable memory behavior with no progressive growth.

### Heavy Mixed Operations
```
Enqueued : 131,103
Dequeued : 131,103
Errors   : 0
```
Mixed workloads completed without data loss or execution errors.

### Cancellation Safety
```
Jobs that survived cancellation: 143,358
```
Cancellation scenarios completed without corrupting store state.

## Test Suites

### CopilotStressTests
- `Concurrent_Producer_Consumer_Should_Not_Lose_Jobs`
- `FIFO_Order_Should_Be_Preserved`
- `Hot_Queue_Contention_Should_Not_Lose_Jobs`
- `Massive_Enqueue_Dequeue_Should_Process_All_Jobs`
- `Memory_Should_Not_Explode`
- `Mixed_Load_For_One_Minute`
- `One_Million_Jobs`
- `Producer_Consumer_For_One_Minute`

### GrokStressTests
- `Cancellation_Does_Not_Corrupt_Store`
- `Concurrent_Producer_Consumer_High_Load`
- `Five_Million_Jobs_Fully_Concurrent`
- `Memory_Pressure_Fill_And_Drain_Multiple_Cycles`
- `Mixed_Operations_Under_Heavy_Load`
- `Sustained_Load_For_Two_Minutes`

## Coverage

- ✓ Queue Integrity  
- ✓ FIFO Ordering  
- ✓ Concurrent Producers / Consumers  
- ✓ High Contention  
- ✓ Mixed Workloads  
- ✓ Sustained Load  
- ✓ Cancellation Handling  
- ✓ Memory Stability  
- ✓ Million-Scale Processing  
- ✓ Multi-Million Concurrent Processing  

## Running

```bash
dotnet test test/WJb.StressTests
```

Run a specific test:

```bash
dotnet test --filter FullyQualifiedName~Five_Million_Jobs_Fully_Concurrent
```

## Philosophy

This suite focuses on **practical stress testing**.

**Goals:**
- Validate real failure modes
- Reproduce issues reliably
- Collect useful operational metrics
- Prevent regressions

**Useful metrics include:**
- Produced
- Consumed
- Difference
- Throughput
- Elapsed
- Memory
- Errors

> The best stress tests explain not only *what* failed, but also *how* the system behaved under load.
