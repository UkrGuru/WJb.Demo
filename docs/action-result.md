# ActionResult

`ActionResult` describes the outcome of an action.

Every action returns an `ActionResult`.

```text
Action
   ↓
ActionResult
```

The result tells WJb:

- Did the action succeed?
- Should another job run?
- Should a value be stored?

---

## Returning Nothing

Use `ActionResults.None()` when the action has completed successfully and no additional information is required.

```csharp
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
```

---

## Returning a Result

Actions can return a value.

```csharp
return ActionResults.Result(
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
return ActionResults.Result(123);
```

Stored result:

```json
123
```

```csharp
return ActionResults.Result("done");
```

Stored result:

```json
"done"
```

```csharp
return ActionResults.Result(true);
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
return ActionResults.Next(
    new JobCommand(
        "log",
        new LogInput
        {
            Message = "Completed"
        }));
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
return ActionResults.Next(
    new JobCommand(
        "email",
        new EmailInput
        {
            To = customer.Email
        }),
    new JobCommand(
        "audit",
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

## ActionResult API

```csharp
public sealed class ActionResult
{
    public object? Value { get; init; }

    public bool Failed { get; init; }

    public IEnumerable<JobCommand> Commands { get; init; }
}
```

Most applications use the helper methods from `ActionResults`.

---

## ActionResults.None

```csharp
return ActionResults.None();
```

Produces:

```text
Success
No Result
No Commands
```

---

## ActionResults.Result

```csharp
return ActionResults.Result(value);
```

Produces:

```text
Success
Result Stored
No Commands
```

Example:

```csharp
return ActionResults.Result(
    new
    {
        OrderId = order.Id
    });
```

---

## ActionResults.Next

```csharp
return ActionResults.Next(
    new JobCommand(
        "send-email",
        email));
```

Produces:

```text
Success
No Result
Next Command
```

---

## Result and Commands Together

An action can return a result and schedule additional jobs.

```csharp
return new ActionResult
{
    Value = new
    {
        Success = true
    },
    Commands =
    [
        new JobCommand(
            "audit",
            new AuditInput())
    ]
};
```

Workflow:

```text
Action
  │
  ├─ Store Result
  │
  └─ Schedule audit
```

---

## Failures

Actions should normally fail by throwing exceptions.

```csharp
throw new InvalidOperationException(
    "SMTP server unavailable");
```

WJb records the failure and stores the error information.

---

## Best Practices

✅ Return meaningful results

✅ Schedule explicit next steps

✅ Use strongly typed payloads

✅ Keep results small

✅ Keep workflows visible

❌ Hide workflow logic

❌ Store large files in results

❌ Create unnecessary wrapper objects

❌ Depend on side effects to drive workflows

---

## Mental Model

```text
Action       = Work

ActionResult = Outcome

JobCommand   = Next Work
```

An action does not directly execute another action.

An action returns an `ActionResult`.

The `ActionResult` describes what happens next.

---

## Source Code

Documentation examples are verified by automated documentation tests.

Tests:

-[../test/WJb.DocTests/02 ActionResultTests.cs](../test/WJb.DocTests/02%20ActionResultTests.cs)