# Actions

Actions contain the business logic of your application.

An action receives input, performs work, and returns an `ActionResult`.

```text
Job
 ↓
Action
 ↓
ActionResult
 ↓
Step
```

---

## Creating an Action

Inherit from `JobAction<TInput>`:

```csharp
[ActionName("send-email")]
private sealed class SendEmailAction : JobAction<EmailInput>
{
    public override ValueTask<ActionResult> ExecuteAsync(
        EmailInput? input, CancellationToken ct = default)
    {
        // business logic...

        return ActionResult.Empty;
    }
}
```

---

## Registering Actions

```csharp
var wjb = WJbBuilder.Create(store, cfg =>
{
    cfg.AddAction<SendEmailAction>("send-email");
});
```

The action key is used when enqueueing jobs:

```csharp
await wjb.EnqueueAsync("send-email",
    new EmailInput { To = "user@test.com" });
```

---

## Action Names

Actions may define explicit names:

```csharp
[ActionName("send-email")]
public sealed class SendEmailAction : JobAction<EmailInput>
{
}
```

This name is automatically used when scheduling next steps:

```csharp
new ActionResults()
    .Next("send-email", payload);
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
return ActionResult.Empty;
```

### Return a Value

```csharp
return new ActionResults()
    .Done(new { Sent = true, Count = 1 });
```
```

The value becomes the job result.

Scalar values are also supported:

```csharp
return new ActionResults(123);
```

```csharp
return new ActionResults("done");
```

```csharp
return new ActionResults(true);
```

---

## Scheduling the Next Step

Actions can schedule the next step using the fluent API.

```csharp
return new ActionResults()
    .Next("log", new LogInput { Message = "Email sent" });
```

Or with a simple payload:

```csharp
return new ActionResults()
    .Next("log", $"Email sent to {input?.To}");
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
return new ActionResults()
    .Next("log", new LogInput { Message = "Email sent" })
    .Next("audit", new AuditInput { Event = "email" });
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
public sealed class SendEmailAction : JobAction<EmailInput>
{
    public override ValueTask<ActionResult> ExecuteAsync(
        EmailInput? input, CancellationToken ct = default)
    {
        return new ActionResults()
            .Next("log", $"Email sent to {input?.To}"));
    }
}

[ActionName("log")]
public sealed class LogAction : JobAction<string?>
{
    public override ValueTask<ActionResult> ExecuteAsync(
        string? message, CancellationToken ct = default)
    {
        Console.WriteLine(message);

        return ActionResults.Empty;
    }
}
```

The workflow is explicit.

The action decides what happens next.

---

## Dependency Injection

Actions support constructor injection.

```csharp
public sealed class SendEmailAction(IEmailService email) : JobAction<EmailInput>
{
    public override ValueTask<ActionResult> ExecuteAsync(
        EmailInput? input, CancellationToken ct = default)
    {
        // await email.SendAsync(...);

        return ActionResults.Empty;
    }
}
```

---

## Failure Handling

Throw an exception when the action cannot complete.

```csharp
public override ValueTask<ActionResult> ExecuteAsync(
    EmailInput? input, CancellationToken ct = default)
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
await httpClient.GetAsync(url, ct);
```

```csharp
await repository.SaveAsync(entity, ct);
```

---

## Best Practices

✅ One business operation per action  
✅ Small input models  
✅ Explicit next steps  
✅ Prefer fluent `.Next(...)`  
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
ActionResult  = Outcome + Next Steps
Step          = Next Work
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
