# FAQ

## Why Another Background Job Library?

Most job systems focus on infrastructure.

WJb focuses on workflow visibility.

Instead of asking:

```text
How does this run?
```

you can answer:

```text
Which action scheduled it?
```

by reading the code.

---

## How Is WJb Different From Hangfire?

Hangfire is built around jobs and infrastructure.

WJb is built around explicit workflows.

Typical WJb workflow:

```text
Action
   ↓
ActionResult
   ↓
Step
   ↓
Next Job
```

The workflow is defined directly in code.

---

## How Is WJb Different From Quartz?

Quartz is primarily a scheduling engine.

WJb is a workflow execution engine.

Quartz answers:

```text
When should this run?
```

WJb answers:

```text
What should happen next?
```

---

## Is WJb a Workflow Engine?

Not in the traditional BPMN sense.

WJb builds workflows using actions and steps.

Example:

```text
create-order
      ↓
send-email
      ↓
audit
      ↓
done
```

No visual designer is required.

The workflow is normal application code.

---

## Can I Schedule Jobs?

Yes.

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload,
    new JobOptions
    {
        Delay = TimeSpan.FromMinutes(5)
    });
```

You can also calculate the execution time:

```csharp
var options = new JobOptions
{
    Delay = TimeSpan.FromHours(1)
};

var runAt = options.GetRunAt(DateTime.UtcNow);
```

---

## Does WJb Support Queues?

Yes.

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload,
    new JobOptions
    {
        Queue = "email"
    });
```

Workers can process specific queues.

```csharp
await wjb.ExecuteLoopAsync(queue: "email");
```

---

## Can Actions Schedule More Jobs?

Yes.

```csharp
return ValueTask.FromResult(
    Results.Done()
        .Next("audit", payload));
```

Actions define workflow transitions.

---

## Can Actions Return Data?

Yes.

```csharp
return ValueTask.FromResult(
    Results.Done(new
    {
        Success = true
    }));
```

Scalar values are also supported.

```csharp
return ValueTask.FromResult(Results.Done(123));
```

```csharp
return ValueTask.FromResult(Results.Done("done"));
```

---

## How Are Failures Handled?

Actions fail by throwing exceptions.

```csharp
throw new InvalidOperationException(
    "SMTP unavailable");
```

WJb stores the failure information and marks the job as failed.

---

## Does WJb Retry Automatically?

Yes.

Retries can be configured using `JobOptions`.

```csharp
new JobOptions
{
    MaxRetries = 3,
    RetryDelay = TimeSpan.FromMinutes(1)
}
```

Exponential backoff is also supported.

```csharp
new JobOptions
{
    MaxRetries = 5,
    RetryDelay = TimeSpan.FromSeconds(5),
    ExponentialBackoff = true
}
```

Retry behavior remains explicit because it is configured directly in application code.

---

## Does WJb Support Dependency Injection?

Yes.

```csharp
public sealed class SendEmailAction(IEmailService email)
    : JobAction<EmailInput>
{
    public override ValueTask<ActionResult> ExecuteAsync(
        EmailInput? input,
        CancellationToken ct = default)
    {
        // ...
        return ValueTask.FromResult(Results.Done());
    }
}
```

Actions are created using dependency injection.

---

## Can I Use My Own Storage Provider?

Yes.

WJb is storage-agnostic.

Examples:

```text
SQL Server
PostgreSQL
SQLite
Redis
MongoDB
IndexedDB
```

You can implement your own store (implement `IStore` or inherit from `StoreBase` / `DbStoreBase`).

---

## Does WJb Require SQL Server?

No.

The core package has no dependency on SQL Server.

Commercial storage providers (SQL Server, PostgreSQL, MySQL, SQLite, IndexedDB) are available separately.

---

## Can Job IDs Be Something Other Than GUIDs?

Yes.

Job identifiers are strings.

```csharp
public string Id { get; init; }
```

A store may internally use:

```text
Guid
long
ULID
ObjectId
```

or any other identifier format.

---

## Can Payloads Be Large?

Yes, but it is usually better to store large content separately.

Preferred:

```csharp
await wjb.EnqueueAsync(
    "send-email",
    new
    {
        BodyId = fileId
    });
```

Instead of:

```csharp
await wjb.EnqueueAsync(
    "send-email",
    new
    {
        Body = hugeHtmlDocument
    });
```

---

## Does WJb Support Progress Reporting?

Yes.

```csharp
ReportProgress(50, "Processing records");
```

Progress and messages can be displayed by monitoring tools.

---

## Is WJb Distributed?

Yes.

A storage provider may be shared across multiple processes or servers.

Distribution depends on the selected store implementation.

---

## Is WJb Open Source?

The core package is available under a non-commercial license.

Commercial use and commercial storage providers require a commercial license.

---

## When Should I Use WJb?

Use WJb when you want to answer:

- Why did this job run?
- What did it do?
- What will run next?
- Why was it retried?

by reading application code.

---

## Mental Model

```text
Action
   ↓
ActionResult
   ↓
Step
   ↓
Next Job
```

If the workflow is visible, it is easier to maintain.

That is the core idea behind WJb.

---

## Source Code

Documentation examples are verified by automated documentation tests.

Tests:

```text
../test/WJb.DocTests/13_FaqTests.cs
```
