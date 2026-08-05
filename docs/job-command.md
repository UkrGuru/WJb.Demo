# JobCommand

A `JobCommand` describes what should happen after an action completes.

It is the building block of workflows in WJb.

```text
Action
   ↓
ActionResult
   ↓
JobCommand
   ↓
New Job
```

Unlike pipeline-based systems, WJb does not hide workflow transitions.

Every scheduled step is represented by a `JobCommand`.

---

## Creating a JobCommand

```csharp
new JobCommand(
    "send-email",
    new EmailInput
    {
        To = "user@test.com"
    });
```

The command specifies:

- Action key
- Payload
- Execution condition

---

## Scheduling the Next Job

Actions can return commands using `ActionResults.Next`.

```csharp
public override Task<ActionResult> ExecuteAsync(
    OrderInput input,
    CancellationToken ct)
{
    return Task.FromResult(
        ActionResults.Next(
            new JobCommand(
                "send-email",
                new EmailInput
                {
                    To = input.Email
                })));
}
```

Workflow:

```text
process-order
        ↓
send-email
```

---

## Multiple Commands

An action can schedule more than one next step.

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
process-order
                └─→ audit
```

---

## Payload

The payload can be any serializable object.

```csharp
new JobCommand(
    "generate-report",
    new ReportInput
    {
        Month = 7,
        Year = 2026
    });
```

Anonymous objects are also supported.

```csharp
new JobCommand(
    "log",
    new
    {
        Message = "Completed"
    });
```

Payloads are automatically serialized and stored with the command.

---

## Command Conditions

### Success (Default)

```csharp
new JobCommand(
    "send-email",
    email);
```

Equivalent:

```csharp
new JobCommand(
    "send-email",
    email,
    JobCommandCondition.Success);
```

The command runs when the action completes successfully.

### Failure

```csharp
new JobCommand(
    "notify-admin",
    notification,
    JobCommandCondition.Failure);
```

The command runs when the action fails.

---

## Helper Methods

WJb provides helper methods for common scenarios.

### JobCommands.Next

```csharp
JobCommands.Next(
    "send-email",
    email);
```

Produces:

```csharp
new JobCommand(
    "send-email",
    email,
    JobCommandCondition.Success);
```

### JobCommands.OnFailure

```csharp
JobCommands.OnFailure(
    "notify-admin",
    notification);
```

Produces:

```csharp
new JobCommand(
    "notify-admin",
    notification,
    JobCommandCondition.Failure);
```

---

## Accessing Payloads

Payloads can be converted back into strongly typed models.

```csharp
var email = command.GetPayload<EmailInput>();
```

For object payloads:

```csharp
var payload = command.AsObject();
```

---

## Chained Workflows

Workflows are built by actions creating commands.

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
CreateOrderAction
    ↓
JobCommand("send-email")

SendEmailAction
    ↓
JobCommand("audit")

AuditAction
    ↓
ActionResults.None()
```

No external workflow configuration is required.

The workflow is defined directly in code.

---

## Best Practices

✅ Schedule explicit next steps

✅ Use strongly typed payloads

✅ Keep commands focused

✅ Use command conditions intentionally

✅ Keep workflow transitions visible

❌ Hidden transitions

❌ Workflow definitions outside code

❌ Large payloads

❌ Commands that perform business logic

---

## Mental Model

```text
Action     = Work

Result     = Outcome

JobCommand = Next Work
```

A `JobCommand` does not execute anything.

A `JobCommand` only describes what should run next.

That explicit transition is what makes workflows easy to understand, debug, and maintain.