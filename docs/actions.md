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
JobCommand
```

---

## Creating an Action

Inherit from `JobAction<TInput>`:

```csharp
public sealed class SendEmailAction : JobAction<EmailInput>
{
    public override async Task<ActionResult> ExecuteAsync(
        EmailInput input,
        CancellationToken ct)
    {
        await _email.SendAsync(
            input.To,
            input.Subject,
            input.Body,
            ct);

        return ActionResults.None();
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
        cfg.AddAction<SendEmailAction>("send-email");
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

## Input Models

Actions can use strongly typed input models.

```csharp
public sealed class EmailInput
{
    public string To { get; init; } = "";

    public string Subject { get; init; } = "";

    public string Body { get; init; } = "";
}
```

WJb automatically converts job payloads into the action input type.

---

## Returning Results

### No Result

```csharp
return ActionResults.None();
```

### Return a Value

```csharp
return ActionResults.Result(
    new
    {
        Sent = true,
        Count = 1
    });
```

The value becomes the job result.

Scalar values are also supported:

```csharp
return ActionResults.Result(123);
```

```csharp
return ActionResults.Result("done");
```

---

## Scheduling the Next Step

Actions can schedule additional jobs.

```csharp
return ActionResults.Next(
    new JobCommand(
        "log",
        new LogInput
        {
            Message = "Email sent"
        }));
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
return ActionResults.Next(
    new JobCommand(
        "log",
        new LogInput
        {
            Message = "Email sent"
        }),
    new JobCommand(
        "audit",
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
public sealed class SendEmailAction : JobAction<EmailInput>
{
    public override Task<ActionResult> ExecuteAsync(
        EmailInput input,
        CancellationToken ct)
    {
        return Task.FromResult(
            ActionResults.Next(
                new JobCommand(
                    "log",
                    new LogInput
                    {
                        Message = $"Email sent to {input.To}"
                    })));
    }
}

public sealed class LogAction : JobAction<LogInput>
{
    public override Task<ActionResult> ExecuteAsync(
        LogInput input,
        CancellationToken ct)
    {
        Console.WriteLine(input.Message);

        return Task.FromResult(
            ActionResults.None());
    }
}
```

The workflow is explicit.

The action decides what happens next.

---

## Dependency Injection

Actions support constructor injection.

```csharp
public sealed class SendEmailAction : JobAction<EmailInput>
{
    private readonly IEmailService _email;

    public SendEmailAction(IEmailService email)
    {
        _email = email;
    }

    public override async Task<ActionResult> ExecuteAsync(
        EmailInput input,
        CancellationToken ct)
    {
        await _email.SendAsync(
            input.To,
            input.Subject,
            input.Body,
            ct);

        return ActionResults.None();
    }
}
```

---

## Failure Handling

Throw an exception when the action cannot complete.

```csharp
public override Task<ActionResult> ExecuteAsync(
    EmailInput input,
    CancellationToken ct)
{
    throw new InvalidOperationException(
        "SMTP server unavailable");
}
```

WJb records the failure and stores the error information.

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
Action = Business Logic

Input  = Work To Perform

Result = Outcome

Command = Next Job
```

If you can read an action and immediately answer:

- What does it do?
- What can it return?
- What runs next?

then the workflow is explicit.

That is the core idea behind WJb.