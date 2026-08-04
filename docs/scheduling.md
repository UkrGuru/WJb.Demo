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

## Scheduled Time

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

Workflow:

```text
Now
 ↓
13:00 UTC
 ↓
send-email
```

The job will not be dequeued before its scheduled time.

---

## Scheduling From Actions

Actions can schedule future work.

```csharp
return ActionResults.Next(
    new JobCommand(
        "send-reminder",
        payload,
        new JobOptions
        {
            Delay = TimeSpan.FromDays(1)
        }));
```

Workflow:

```text
create-account
       ↓
Wait 1 Day
       ↓
send-reminder
```

---

## Reminder Workflow

```text
register-user
       ↓
welcome-email
       ↓
wait 7 days
       ↓
follow-up-email
```

```csharp
return ActionResults.Next(
    new JobCommand(
        "follow-up-email",
        input,
        new JobOptions
        {
            Delay = TimeSpan.FromDays(7)
        }));
```

---

## Multiple Scheduled Jobs

An action may schedule several future jobs.

```csharp
return ActionResults.Next(
    new JobCommand(
        "email",
        payload,
        new JobOptions
        {
            Delay = TimeSpan.FromMinutes(5)
        }),
    new JobCommand(
        "audit",
        payload,
        new JobOptions
        {
            Delay = TimeSpan.FromHours(1)
        }));
```

Workflow:

```text
              ┌─→ email (5 min)
current-job
              └─→ audit (1 hour)
```

---

## Scheduling vs Queues

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

✅ Store schedule information inside workflows

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