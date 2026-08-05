# Scheduling

Scheduling controls when a job becomes available for execution.

```text
Now
 ↓
Schedule
 ↓
Future Job
```

A scheduled job is stored immediately but cannot execute until its scheduled time.

---

## Immediate Execution

By default, jobs are available immediately.

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload);
```

Workflow:

```text
Now
 ↓
send-email
```

---

## Delayed Execution

Use a delay when work should happen later.

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload,
    new JobOptions
    {
        Delay = TimeSpan.FromMinutes(10)
    });
```

Workflow:

```text
Now
 ↓
Wait 10 Minutes
 ↓
send-email
```

---

## How Scheduling Works

WJb calculates the execution time from the current time and the configured delay.

```csharp
var options = new JobOptions
{
    Delay = TimeSpan.FromHours(1)
};
```

The job becomes available one hour later.

---

## Scheduling and Queues

Scheduling determines:

```text
When
```

Queues determine:

```text
Where
```

Example:

```csharp
new JobOptions
{
    Delay = TimeSpan.FromMinutes(30),
    Queue = "email"
}
```

Meaning:

```text
Wait 30 Minutes
       ↓
Email Queue
```

---

## Reminder Workflow

```text
register-user
       ↓
wait 7 days
       ↓
follow-up-email
```

Enqueue:

```csharp
await wjb.EnqueueAsync(
    "follow-up-email",
    payload,
    new JobOptions
    {
        Delay = TimeSpan.FromDays(7)
    });
```

---

## Common Use Cases

### Follow-Up Emails

```text
Order Created
      ↓
7 Days
      ↓
Feedback Request
```

---

### Subscription Reminders

```text
Subscription Expires
         ↓
3 Days
         ↓
Reminder Email
```

---

### Cleanup Jobs

```text
Upload Complete
        ↓
24 Hours
        ↓
Delete Temporary Files
```

---

## Best Practices

✅ Schedule business events explicitly

✅ Use delays for future work

✅ Keep scheduling logic visible

✅ Keep delay values close to workflow code

❌ Hide scheduling in infrastructure

❌ Use timers scattered across the application

❌ Depend on background threads for future execution

---

## Mental Model

```text
Action = What

Schedule = When

Queue = Where
```

Scheduling answers a single question:

> When should this job become available for execution?

---

## Source Code

Documentation examples are verified by automated documentation tests.

Tests:

-[../test/WJb.DocTests/08 SchedulingTests.cs](../test/WJb.DocTests/08%20SchedulingTests.cs)