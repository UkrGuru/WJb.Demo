# JobOptions

`JobOptions` controls how a job is scheduled and retried.

```text
Job
 ↓
JobOptions
 ↓
Queue
Delay
Retry
```

Job options are optional.

If no options are specified, the job is scheduled immediately.

---

## Immediate Execution

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload);
```

The job becomes available immediately.

---

## Delay

Use a delay to execute a job in the future.

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload,
    new JobOptions
    {
        Delay = TimeSpan.FromMinutes(5)
    });
```

Workflow:

```text
Now
 ↓
Wait 5 Minutes
 ↓
send-email
```

A delay of `TimeSpan.Zero` means immediate execution.

---

## Queue

Queues separate different workloads.

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload,
    new JobOptions
    {
        Queue = "email"
    });
```

Worker:

```csharp
await wjb.ExecuteLoopAsync(
    queue: "email");
```

Only jobs from that queue will execute.

---

## Retries

Configure how many times a failed job may be retried.

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload,
    new JobOptions
    {
        MaxRetries = 3
    });
```

Workflow:

```text
Attempt 1
   ↓
Failure
   ↓
Attempt 2
   ↓
Failure
   ↓
Attempt 3
```

---

## Retry Delay

Use a fixed delay between retry attempts.

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload,
    new JobOptions
    {
        MaxRetries = 3,
        RetryDelay = TimeSpan.FromSeconds(10)
    });
```

Each retry waits 10 seconds.

---

## Exponential Backoff

Retry delays can grow automatically after each failure.

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload,
    new JobOptions
    {
        MaxRetries = 5,
        RetryDelay = TimeSpan.FromSeconds(5),
        ExponentialBackoff = true
    });
```

Retry schedule:

```text
Attempt 1 → 5s
Attempt 2 → 10s
Attempt 3 → 20s
Attempt 4 → 40s
```

---

## Default Values

```csharp
new JobOptions()
```

Defaults:

```text
Queue = null
Delay = 00:00:00
MaxRetries = 0
RetryDelay = 00:00:05
ExponentialBackoff = false
```

---

## Best Practices

✅ Use queues to isolate workloads

✅ Use delays for future work

✅ Configure retries intentionally

✅ Use exponential backoff for transient failures

✅ Keep queue names simple

❌ Use queues as business rules

❌ Create excessive queue counts

❌ Retry non-transient failures forever

---

## Mental Model

```text
Action     = What

Payload    = Data

JobOptions = When / How
```

`JobOptions` does not change business logic.

`JobOptions` only controls scheduling and retry behavior.