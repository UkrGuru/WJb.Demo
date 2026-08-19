# IActionResult

`IActionResult` describes the outcome of an action.

Every action returns an `IActionResult`.

```text
Action
   ↓
IActionResult
```

The result tells WJb:

- Is the workflow complete?
- Should another job run?
- Should a value be stored?

---

## Complete the Workflow

Use `CompleteAsync()` when the action has completed successfully and no additional information is required.

```csharp
public override async Task<IActionResult> ExecuteAsync(
    EmailInput input,
    CancellationToken ct)
{
    await _email.SendAsync(
        input.To,
        input.Subject,
        input.Body,
        ct);

    return await CompleteAsync();
}
```

---

## Complete with a Result

Actions can return a value.

```csharp
return Results.Complete(
    new
    {
        Sent = true,
        Count = 1
    });
```

The value becomes the job result.

Stored result:

```json
{
  "Sent": true,
  "Count": 1
}
```

---

## Scalar Results

Scalar values are fully supported.

```csharp
return Results.Complete(123);
```

Stored result:

```json
123
```

```csharp
return Results.Complete("done");
```

Stored result:

```json
"done"
```

```csharp
return Results.Complete(true);
```

Stored result:

```json
true
```

No wrapper objects are required.

---

## Scheduling the Next Step

Actions can schedule new jobs.

```csharp
return await NextAsync<LogAction>(
    new LogInput
    {
        Message = "Completed"
    });
```

Workflow:

```text
current-action
       ↓
      log
```

---

## Scheduling Multiple Jobs

Multiple commands can be returned.

```csharp
return Results.Next(
    JobCommands.Next<EmailAction>(
        new EmailInput
        {
            To = customer.Email
        }),
    JobCommands.Next<AuditAction>(
        new AuditInput
        {
            Event = "OrderCompleted"
        }));
```

Workflow:

```text
               ┌─→ email
current-action
               └─→ audit
```

---

## CompleteResult

Represents a completed workflow.

```csharp
public sealed class CompleteResult
    : IActionResult
{
    public object? Value { get; }
}
```

Example:

```csharp
return Results.Complete(
    new
    {
        OrderId = order.Id
    });
```

Produces:

```text
Workflow Completed
Result Stored
```

---

## NextResult

Represents one or more workflow continuations.

```csharp
public sealed class NextResult
    : IActionResult
{
    public IReadOnlyList<JobCommand> Commands { get; }
}
```

Example:

```csharp
return Results.Next(
    JobCommands.Next<SendEmailAction>(
        email));
```

Produces:

```text
Workflow Continues
Next Command Scheduled
```

---

## Failures

Actions should normally fail by throwing exceptions.

```csharp
throw new InvalidOperationException(
    "SMTP server unavailable");
```

WJb records the failure and stores error information.

---

## Best Practices

✅ Return meaningful results

✅ Schedule explicit next steps

✅ Use strongly typed payloads

✅ Prefer `NextAsync<TAction>()`

✅ Keep workflows visible

❌ Hide workflow logic

❌ Store large files in results

❌ Depend on side effects to drive workflows

---

## Mental Model

```text
Action         = Work

IActionResult  = Outcome

JobCommand     = Next Work
```

An action does not execute another action.

An action returns an `IActionResult`.

The `IActionResult` describes what happens next.

---

## Source Code

Documentation examples are verified by automated documentation tests.

Tests:

```text
../test/WJb.DocTests/02_ActionResultTests.cs
```
