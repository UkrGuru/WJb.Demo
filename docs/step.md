# Step

A `Step` describes what should happen after an action completes.

It is the building block of workflows in WJb.

```text
Action
   ↓
ActionResult
   ↓
Step
   ↓
New Job
```

Unlike pipeline-based systems, WJb does not hide workflow transitions.

Every scheduled step is represented by a `Step`.

---

## Creating a Step

The most common way is through the fluent API on `ActionResult`:

```csharp
Results.Done()
    .Next("send-email", new EmailInput
    {
        To = "user@test.com"
    });
```

You can also work with `Step` / `Step<TAction>` directly when needed.

A step specifies:

- Action key (or typed action)
- Payload
- Optional execution condition (`StepCondition`)

---

## Scheduling the Next Job

Actions schedule the next step using `.Next(...)`:

```csharp
public override ValueTask<ActionResult> ExecuteAsync(
    OrderInput? input,
    CancellationToken ct = default)
{
    return ValueTask.FromResult(
        Results.Done()
            .Next("send-email", new EmailInput
            {
                To = input?.Email
            }));
}
```

Workflow:

```text
process-order
        ↓
send-email
```

---

## Multiple Steps

An action can schedule more than one next step by chaining `.Next(...)`:

```csharp
return ValueTask.FromResult(
    Results.Done()
        .Next("email", new EmailInput { To = customer.Email })
        .Next("audit", new AuditInput { Event = "OrderCompleted" }));
```

Workflow:

```text
                ┌─→ email
process-order
                └─→ audit
```

---

## Payload

The payload can be any serializable object.

```csharp
Results.Done()
    .Next("generate-report", new ReportInput
    {
        Month = 7,
        Year = 2026
    });
```

Anonymous objects are also supported.

```csharp
Results.Done()
    .Next("log", new
    {
        Message = "Completed"
    });
```

Payloads are automatically serialized and stored with the step.

---

## Step Conditions

### Success (Default)

```csharp
Results.Done()
    .Next("send-email", email);
```

The step runs when the action completes successfully.

### Failure

Steps can be conditioned using `StepCondition` (e.g. only on failure).

```csharp
// Conceptual example
// Step with StepCondition.Failure
```

The step runs when the action fails.

---

## Typed Steps

You can use the typed variant `Step<TAction>`:

```csharp
// Prefer the fluent API in most cases
Results.Done()
    .Next<SendEmailAction>(email);
```

(When the action has an `[ActionName]` attribute, the key is resolved automatically.)

---

## Chained Workflows

Workflows are built by actions creating steps.

```text
create-order
      ↓
send-email
      ↓
audit
      ↓
done
```

```csharp
// CreateOrderAction
return ValueTask.FromResult(
    Results.Done().Next("send-email", emailPayload));

// SendEmailAction
return ValueTask.FromResult(
    Results.Done().Next("audit", auditPayload));

// AuditAction
return ValueTask.FromResult(Results.Done());
```

No external workflow configuration is required.

The workflow is defined directly in code.

---

## Best Practices

✅ Schedule explicit next steps  
✅ Use strongly typed payloads  
✅ Prefer the fluent `.Next(...)` API  
✅ Keep steps focused  
✅ Use step conditions intentionally  
✅ Keep workflow transitions visible  

❌ Hidden transitions  
❌ Workflow definitions outside code  
❌ Large payloads  
❌ Steps that perform business logic  

---

## Mental Model

```text
Action        = Work
ActionResult  = Outcome + Next Steps
Step          = Next Work
```

A `Step` does **not** execute anything.

A `Step` only describes what should run next.

That explicit transition is what makes workflows easy to understand, debug, and maintain.

---

## Source Code

Documentation examples are verified by automated documentation tests.

Tests:

```text
../test/WJb.DocTests/03_StepTests.cs
```
