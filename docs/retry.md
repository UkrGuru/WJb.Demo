# Retry

Failures happen.

Networks fail.  
Databases become unavailable.  
External APIs timeout.

A retry allows a job to be attempted again.

```text
Job
 ↓
Fail
 ↓
Retry
 ↓
Success
```

Retry behavior in WJb is **explicit**.

---

## Why Explicit Retries?

Many systems hide retry behavior inside infrastructure.

```text
Job
 ↓
???
 ↓
Retry
```

WJb keeps retries visible.

```text
Job
 ↓
Action
 ↓
Step
 ↓
Retry
```

You can see where and why the retry was created.

---

## Configuring Retries

Retries are configured using `JobOptions`.

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

Use a delay between retry attempts.

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload,
    new JobOptions
    {
        MaxRetries = 3,
        RetryDelay = TimeSpan.FromMinutes(1)
    });
```

Each retry waits one minute before the next attempt.

---

## Exponential Backoff

Retry delays can increase automatically after each failure.

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload,
    new JobOptions
    {
        MaxRetries = 5,
        RetryDelay = TimeSpan.FromMinutes(2),
        ExponentialBackoff = true
    });
```

Produces:

```text
Attempt 1 → 2 min
Attempt 2 → 4 min
Attempt 3 → 8 min
Attempt 4 → 16 min
```

---

## Retry Counter

Applications can track attempts in the payload when needed.

Example payload:

```csharp
public sealed class EmailInput
{
    public string To { get; init; } = "";
    public int Attempt { get; init; }
}
```

Example:

```csharp
var next = input with
{
    Attempt = input.Attempt + 1
};
```

---

## Maximum Attempts

Prevent infinite retries.

```csharp
new JobOptions
{
    MaxRetries = 5
}
```

Example workflow:

```text
Attempt 1
    ↓
Attempt 2
    ↓
Attempt 3
    ↓
Attempt 4
    ↓
Attempt 5
    ↓
Stop
```

---

## Failure Processing

Retries are not the only option.

A failed action can trigger follow-up work using step conditions (e.g. `StepCondition.Failure`).

Workflow:

```text
send-email
      ↓
failure
      ↓
notify-admin
```

---

## Manual / Explicit Retry from Action

A common pattern when you want full control:

```csharp
public sealed class SendEmailAction
    : JobAction<EmailInput>
{
    public override async ValueTask<ActionResult> ExecuteAsync(
        EmailInput? input,
        CancellationToken ct = default)
    {
        try
        {
            await SendAsync(input, ct);
            return Results.Done();
        }
        catch
        {
            return Results.Done()
                .Next("send-email", input, new JobOptions
                {
                    Delay = TimeSpan.FromMinutes(5)
                });
        }
    }
}
```

The workflow is visible.  
No hidden retry engine is involved.

---

## Retry Counter (Manual)

```csharp
return Results.Done()
    .Next("send-email", input with
    {
        Attempt = input.Attempt + 1
    });
```

---

## Maximum Attempts (Manual)

```csharp
if (input.Attempt >= 5)
{
    return Results.Done();
}
```

---

## Exponential Backoff (Manual)

```csharp
var delay = TimeSpan.FromMinutes(
    Math.Pow(2, input.Attempt));
```

Produces:

```text
Attempt 1 → 2 min
Attempt 2 → 4 min
Attempt 3 → 8 min
Attempt 4 → 16 min
```

---

## Retry Another Action

Retries do not have to schedule the same action.

```csharp
return Results.Done()
    .Next("notify-admin", input);
```

Workflow:

```text
send-email
      ↓
notify-admin
```

---

## Explicit Failure Processing

Alternative workflow:

```text
send-email
      ↓
email-failed
      ↓
notify-admin
```

```csharp
return Results.Done()
    .Next("email-failed", input);
```

---

## Hidden Retry vs Explicit Retry

**Hidden:**

```text
Fail
 ↓
Infrastructure
 ↓
Retry
```

**Explicit (WJb):**

```text
Fail
 ↓
Action / JobOptions
 ↓
Step
 ↓
Retry
```

The explicit approach makes workflows easier to understand.

---

## Best Practices

✅ Limit retry attempts  
✅ Use delays  
✅ Use exponential backoff  
✅ Make retries visible  
✅ Track attempt counts  

❌ Infinite retries  
❌ Hidden retry logic  
❌ Immediate retry loops  
❌ Retry everything automatically  

---

## Mental Model

```text
Failure = Event
Retry   = Decision
Job     = Next Attempt
```

A retry is not magic.  
A retry is simply another job.

---

## Source Code

Documentation examples are verified by automated documentation tests.

Tests:

```text
../test/WJb.DocTests/07_RetryTests.cs
```
