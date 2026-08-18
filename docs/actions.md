# Actions

Actions contain the business logic of your application.

An action receives input, performs work, and returns an `IActionResult`.

```text
Job
 ↓
Action
 ↓
IActionResult
 ↓
JobCommand
```

---

## Creating an Action

Inherit from `JobAction<TInput>`:

```csharp
public sealed class SendEmailAction
    : JobAction<EmailInput>
{
    public override async Task<IActionResult> ExecuteAsync(
        EmailInput input,
        CancellationToken ct)
    {
        await Task.CompletedTask;

        return await CompleteAsync();
    }
}
```

---

## Registering Actions

```csharp
var wjb = WJbBuilder.Create(
    store,
    cfg =>
    {
        cfg.AddAction<SendEmailAction>(
            "send-email");
    });
```

The action key is used when enqueueing jobs:

```csharp
await wjb.EnqueueAsync(
    "send-email",
    new EmailInput
    {
        To = "user@test.com"
    });
```

---

## Action Names

Actions may define explicit names:

```csharp
[ActionName("send-email")]
public sealed class SendEmailAction
    : JobAction<EmailInput>
{
}
```

This name is automatically used by:

```csharp
JobCommands.Next<SendEmailAction>()
```

and

```csharp
NextAsync<SendEmailAction>()
```

---

## Input Models

Actions can use strongly typed input models.

```csharp
public sealed class EmailInput
{
    public string To { get; init; } = string.Empty;

    public string Subject { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;
}
```

WJb automatically converts job payloads into the action input type.

---

## Completing a Workflow

### No Result

```csharp
return await CompleteAsync();
```

### Return a Value

```csharp
return Results.Complete(
    new
    {
        Sent = true,
        Count = 1
    });
```

The value becomes the job result.

Scalar values are also supported:

```csharp
return Results.Complete(123);
```

```csharp
return Results.Complete("done");
```

```csharp
return Results.Complete(true);
```

---

## Scheduling the Next Step

Actions can schedule additional jobs.

```csharp
return await NextAsync<LogAction>(
    new LogInput
    {
        Message = "Email sent"
    });
```

Workflow:

```text
send-email
      ↓
log
```

---

## Multiple Next Steps

```csharp
return Results.Next(
    JobCommands.Next<LogAction>(
        new LogInput
        {
            Message = "Email sent"
        }),
    JobCommands.Next<AuditAction>(
        new AuditInput
        {
            Event = "email"
        }));
```

Workflow:

```text
          ┌─→ log
send-email
          └─→ audit
```

---

## Workflow Example

```text
send-email
      ↓
log
      ↓
done
```

```csharp
[ActionName("send-email")]
public sealed class SendEmailAction
    : JobAction<EmailInput>
{
    public override async Task<IActionResult> ExecuteAsync(
        EmailInput input,
        CancellationToken ct)
    {
        return await NextAsync<LogAction>(
            new LogInput
            {
                Message =
                    $"Email sent to {input.To}"
            });
    }
}

[ActionName("log")]
public sealed class LogAction
    : JobAction<LogInput>
{
    public override async Task<IActionResult> ExecuteAsync(
        LogInput input,
        CancellationToken ct)
    {
        Console.WriteLine(
            input.Message);

        return await CompleteAsync();
    }
}
```

The workflow is explicit.

The action decides what happens next.

---

## Dependency Injection

Actions support constructor injection.

```csharp
public sealed class SendEmailAction(
    IEmailService email)
    : JobAction<EmailInput>
{
    public override async Task<IActionResult> ExecuteAsync(
        EmailInput input,
        CancellationToken ct)
    {
        await email.SendAsync(
            input.To,
            input.Subject,
            input.Body,
            ct);

        return await CompleteAsync();
    }
}
```

---

## Failure Handling

Throw an exception when the action cannot complete.

```csharp
public override Task<IActionResult> ExecuteAsync(
    EmailInput input,
    CancellationToken ct)
{
    throw new InvalidOperationException(
        "SMTP server unavailable");
}
```

WJb records the failure and stores error information.

---

## Cancellation

Always pass the cancellation token to external operations.

```csharp
await httpClient.GetAsync(
    url,
    ct);
```

```csharp
await repository.SaveAsync(
    entity,
    ct);
```

---

## Best Practices

✅ One business operation per action

✅ Small input models

✅ Explicit next steps

✅ Prefer `NextAsync<TAction>()`

✅ Constructor injection

✅ Return meaningful results

✅ Pass cancellation tokens

✅ Keep actions focused

❌ Hidden workflows

❌ Service locator patterns

❌ Large payloads

❌ Long chains of implicit behavior

---

## Mental Model

```text
Action        = Business Logic

Input         = Work To Perform

IActionResult = Outcome

JobCommand    = Next Job
```

If you can read an action and immediately answer:

- What does it do?
- What can it return?
- What runs next?

then the workflow is explicit.

That is the core idea behind WJb.

---

## Source Code

Documentation examples are verified by automated documentation tests.

Tests:

```text
../test/WJb.DocTests/01_ActionsTests.cs
```
