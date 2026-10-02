# ActionResult

`ActionResult` describes the outcome of an action.

Every action returns an `ActionResult`.

```text
Action
   ↓
ActionResult
```

The result tells WJb:

- Is the workflow complete?
- Should another step run?
- Should a value be stored?

---

## Complete the Workflow

Use `Results.Done()` when the action has completed successfully and no additional information is required.

```csharp
public override ValueTask<ActionResult> ExecuteAsync(
    EmailInput? input,
    CancellationToken ct = default)
{
    // business logic...

    return ValueTask.FromResult(Results.Done());
}
```

---

## Complete with a Result

Actions can return a value.

```csharp
return ValueTask.FromResult(
    Results.Done(new
    {
        Sent = true,
        Count = 1
    }));
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
return ValueTask.FromResult(Results.Done(123));
```

Stored result:

```json
123
```

```csharp
return ValueTask.FromResult(Results.Done("done"));
```

Stored result:

```json
"done"
```

```csharp
return ValueTask.FromResult(Results.Done(true));
```

Stored result:

```json
true
```

No wrapper objects are required.

---

## Scheduling the Next Step

Actions can schedule the next step using the fluent API.

```csharp
return ValueTask.FromResult(
    Results.Done()
        .Next("log", new LogInput
        {
            Message = "Completed"
        }));
```

Or with a simple payload:

```csharp
return ValueTask.FromResult(
    Results.Done()
        .Next("log", $"Email sent to {input?.To}"));
```

Workflow:

```text
current-action
       ↓
      log
```

---

## Scheduling Multiple Steps

Multiple next steps can be chained.

```csharp
return ValueTask.FromResult(
    Results.Done()
        .Next("email", new EmailInput { To = customer.Email })
        .Next("audit", new AuditInput { Event = "OrderCompleted" }));
```

Workflow:

```text
               ┌─→ email
current-action
               └─→ audit
```

---

## ActionResult

Represents the outcome of an action (completion + optional next steps).

```csharp
// Conceptual shape
public class ActionResult
{
    public object? Value { get; }
    // + next Steps
}
```

Example:

```csharp
return ValueTask.FromResult(
    Results.Done(new { OrderId = order.Id }));
```

Produces:

```text
Workflow Completed
Result Stored
```

---

## Step

Represents a single workflow continuation.

```csharp
// Conceptual
public class Step
{
    // Action key + payload + optional condition
}
```

Typed variant also exists: `Step<TAction>`.

Example:

```csharp
Results.Done()
    .Next("send-email", email);
```

Produces:

```text
Workflow Continues
Next Step Scheduled
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
✅ Prefer fluent `.Next(...)`  
✅ Keep workflows visible  

❌ Hide workflow logic  
❌ Store large files in results  
❌ Depend on side effects to drive workflows  

---

## Mental Model

```text
Action        = Work
ActionResult  = Outcome + Next Steps
Step          = Next Work
```

An action does **not** execute another action.

An action returns an `ActionResult`.

The `ActionResult` (via `Results.Done().Next(...)`) describes what happens next.

---

## Source Code

Documentation examples are verified by automated documentation tests.

Tests:

```text
../test/WJb.DocTests/...
```
