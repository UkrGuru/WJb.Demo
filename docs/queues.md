# Queues

> // Available only in the commercial edition.

Queues allow jobs to be separated by workload.

Instead of processing everything through a single worker, jobs can be routed to dedicated queues.

```text
email jobs
     ↓
 email worker

report jobs
     ↓
report worker

import jobs
     ↓
import worker
```

---

## Default Queue

When no queue is specified, a job is placed into the default queue.

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload);
```

No queue configuration is required.

---

## Assigning a Queue

Use `JobOptions.Queue`.

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload,
    new JobOptions
    {
        Queue = "email"
    });
```

The job is stored in the specified queue.

---

## Processing a Queue

```csharp
await wjb.ExecuteLoopAsync(
    queue: "email");
```

Only jobs from the `email` queue will be processed.

---

## Multiple Workers

Different workers can process different queues.

```text
             ┌─────────────┐
email queue  │ EmailWorker │
             └─────────────┘

             ┌──────────────┐
report queue │ ReportWorker │
             └──────────────┘

             ┌──────────────┐
import queue │ ImportWorker │
             └──────────────┘
```

Each worker focuses on a single type of workload.

---

## Why Use Queues?

Queues help isolate workloads.

Example:

```text
100,000 imports
```

should not delay:

```text
password reset email
```

Without queues:

```text
All Jobs
    ↓
Single Worker
```

With queues:

```text
Imports
    ↓
Import Workers

Emails
    ↓
Email Workers
```

---

## Naming Queues

Good queue names:

```text
email
```

```text
reports
```

```text
imports
```

```text
notifications
```

Avoid queue names tied to temporary implementation details.

---

## Queue Strategy

A common approach:

```text
critical

default

background
```

Example:

```text
critical
 ├─ password reset
 ├─ user notifications

default
 ├─ emails
 ├─ webhooks

background
 ├─ reports
 ├─ imports
 ├─ cleanup
```

---

## Queue Isolation

Queues do not change business behavior.

This:

```csharp
Queue = "email"
```

means:

```text
Where It Runs
```

not:

```text
What It Does
```

Business rules belong inside actions.

---

## Best Practices

✅ Separate different workloads

✅ Use simple queue names

✅ Isolate expensive jobs

✅ Scale workers independently

✅ Keep queue design stable

❌ Put business logic into queue names

❌ Create dozens of queues unnecessarily

❌ Route jobs dynamically without a clear reason

❌ Use queues as permission rules

---

## Mental Model

```text
Action = What Runs

Payload = Data

Queue = Where It Runs
```

Queues help organize execution.

They should not define behavior.

---

## Source Code

Documentation examples are verified by automated documentation tests.

Tests:

-[../test/WJb.DocTests/09 QueueTests.cs](../test/WJb.DocTests/09%20QueueTests.cs)