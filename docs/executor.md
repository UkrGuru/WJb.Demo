# Executor

The executor is responsible for running jobs.

```text
Store
  ↓
Executor
  ↓
Action
```

It loads jobs from the store, executes actions, stores results, and schedules follow-up work.

The executor does not contain business logic.

Business logic belongs inside actions.

---

## What the Executor Does

```text
Find Job
    ↓
Load Payload
    ↓
Execute Action
    ↓
Store Result
    ↓
Schedule Commands
    ↓
Complete Job
```

Every job follows the same lifecycle.

---

## Running Once

Execute a single available job.

```csharp
await wjb.ExecuteAsync();
```

If no job is available, nothing happens.

---

## Running Continuously

Run jobs in a loop.

```csharp
await wjb.ExecuteLoopAsync();
```

The executor continuously:

```text
Dequeue
Execute
Complete
Repeat
```

This is the most common production configuration.

---

## Running Continuously

Run jobs in a loop.

```csharp
await wjb.ExecuteLoopAsync();
```

The executor continuously:

```text
Dequeue
Execute
Complete
Repeat
```

This is the most common production configuration.

---

## Queue Execution

> // Available only in the commercial edition.

Execute jobs from a specific queue.

```csharp
await wjb.ExecuteLoopAsync(
    queue: "email");
```

Only jobs assigned to that queue will run.

---

## Job Lifecycle

### Pending

A new job starts in the pending state.

```text
Pending
```

---

### Running

When a worker picks up a job:

```text
Pending
   ↓
Running
```

---

### Completed

Successful execution:

```text
Running
   ↓
Completed
```

Result data is stored.

---

### Failed

If an exception occurs:

```text
Running
   ↓
Failed
```

Failure information is stored.

---

## Executing an Action

Example:

```csharp
public sealed class SendEmailAction
    : JobAction<EmailInput>
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

The executor:

```text
1. Loads payload

2. Creates action

3. Executes action

4. Processes ActionResult
```

---

## Returning Results

```csharp
return ActionResults.Result(
    new
    {
        Success = true
    });
```

The executor stores the returned value as the job result.

---

## Scheduling Commands

```csharp
return ActionResults.Next(
    new JobCommand(
        "audit",
        payload));
```

The executor schedules new jobs.

Workflow:

```text
Current Job
      ↓
  JobCommand
      ↓
   New Job
```

---

## Multiple Commands

```csharp
return ActionResults.Next(
    new JobCommand("email", email),
    new JobCommand("audit", audit));
```

Workflow:

```text
          ┌─→ email
Current
          └─→ audit
```

The executor schedules both jobs.

---

## Exceptions

Actions signal failure by throwing exceptions.

```csharp
throw new InvalidOperationException(
    "SMTP server unavailable");
```

The executor:

```text
Marks Job Failed

Stores Error

Stops Execution
```

---

## Cancellation

Cancellation requests are propagated to actions.

```csharp
public override async Task<ActionResult> ExecuteAsync(
    MyInput input,
    CancellationToken ct)
{
    ct.ThrowIfCancellationRequested();

    await SomeOperationAsync(ct);

    return ActionResults.None();
}
```

Always pass the cancellation token to external operations.

---

## Dependency Injection

The executor creates actions through dependency injection.

```csharp
public sealed class SendEmailAction
    : JobAction<EmailInput>
{
    private readonly IEmailService _email;

    public SendEmailAction(
        IEmailService email)
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

## What the Executor Does Not Do

The executor does not:

❌ Implement business rules

❌ Decide workflow transitions

❌ Modify payloads

❌ Know application-specific logic

Those responsibilities belong to actions.

---

## Best Practices

✅ Keep business logic inside actions

✅ Use explicit commands

✅ Return meaningful results

✅ Pass cancellation tokens

✅ Separate workloads using queues

❌ Hide workflow logic inside infrastructure

❌ Build workflows inside the executor

❌ Depend on side effects

---

## Mental Model

```text
Store
  ↓
Executor
  ↓
Action
  ↓
ActionResult
  ↓
JobCommand
```

The executor runs the workflow.

The action defines the workflow.