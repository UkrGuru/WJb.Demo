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

Retry behavior in WJb is explicit.

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
JobCommand
 ↓
Retry
```

You can see where and why the retry was created.

---

## Simple Retry

```csharp
return ActionResults.Next(
    new JobCommand(
        "send-email",
        input,
        new JobOptions
        {
            Delay = TimeSpan.FromMinutes(1)
        }));
```

Workflow:

```text
send-email
      ↓
wait 1 minute
      ↓
send-email
```

---

## Retry After Failure

A common pattern:

```csharp
public sealed class SendEmailAction
    : JobAction<EmailInput>
{
    public override async Task<ActionResult> ExecuteAsync(
        EmailInput input,
        CancellationToken ct)
    {
        try
        {
            await SendAsync(input, ct);

            return ActionResults.None();
        }
        catch
        {
            return ActionResults.Next(
                new JobCommand(
                    "send-email",
                    input,
                    new JobOptions
                    {
                        Delay = TimeSpan.FromMinutes(5)
                    }));
        }
    }
}
```

The workflow is visible.

No hidden retry engine is involved.

---

## Retry Counter

Applications often track retry attempts.

Example payload:

```csharp
public sealed class EmailInput
{
    public string To { get; init; } = "";

    public int Attempt { get; init; }
}
```

Action:

```csharp
return ActionResults.Next(
    new JobCommand(
        "send-email",
        input with
        {
            Attempt = input.Attempt + 1
        }));
```

---

## Maximum Attempts

Prevent infinite retries.

```csharp
if (input.Attempt >= 5)
{
    return ActionResults.None();
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

## Exponential Backoff

A common strategy:

```csharp
var delay =
    TimeSpan.FromMinutes(
        Math.Pow(
            2,
            input.Attempt));
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

Example:

```csharp
return ActionResults.Next(
    new JobCommand(
        "notify-admin",
        input));
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

Example:

```csharp
return ActionResults.Next(
    new JobCommand(
        "email-failed",
        input));
```

---

## Hidden Retry vs Explicit Retry

Hidden:

```text
Fail
 ↓
Infrastructure
 ↓
Retry
```

Explicit:

```text
Fail
 ↓
Action
 ↓
JobCommand
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