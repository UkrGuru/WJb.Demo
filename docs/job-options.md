# JobOptions

`JobOptions` controls when and where a job executes.

```text
Job
 ↓
JobOptions
 ↓
Queue
Schedule
Delay
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

---

## Schedule

Use a specific timestamp.

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload,
    new JobOptions
    {
        RunAt = DateTime.UtcNow.AddHours(1)
    });
```

The job will not execute before the specified time.

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

## From JobCommand

Options can also be used inside workflow commands.

```csharp
return ActionResults.Next(
    new JobCommand(
        "email",
        payload,
        new JobOptions
        {
            Delay = TimeSpan.FromMinutes(30)
        }));
```

Workflow:

```text
Action
 ↓
Wait 30 Minutes
 ↓
email
```

---

## Best Practices

✅ Use queues to isolate workloads

✅ Use delays for future work

✅ Use explicit scheduling

✅ Keep queue names simple

❌ Use queues as business rules

❌ Create excessive queue counts

---

## Mental Model

```text
Action     = What

Payload    = Data

JobOptions = When / Where
```
