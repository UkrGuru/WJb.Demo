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
- Optional execution options

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

---

## Delayed Execution

Use `JobOptions` to schedule a command in the future.

```csharp
new JobCommand(
    "send-email",
    new EmailInput
    {
        To = "user@test.com"
    },
    new JobOptions
    {
        Delay = TimeSpan.FromMinutes(5)
    });
```

Workflow:

```text
Action
   ↓
Wait 5 minutes
   ↓
send-email
```

---

## Queues

Use queues to route work.

```csharp
new JobCommand(
    "send-email",
    new EmailInput
    {
        To = "user@test.com"
    },
    new JobOptions
    {
        Queue = "email"
    });
```

The job will be scheduled into the specified queue.

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

✅ Use queues intentionally

✅ Use delays only when needed

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