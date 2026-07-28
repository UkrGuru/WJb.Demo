# WJb Integration Tests: Verified Job Types

One of the most common questions from developers evaluating a background job framework is:

> Can it do the same types of jobs as Hangfire?

Instead of answering this theoretically, WJb includes a dedicated **IntegrationTests** project containing real executable scenarios.

Each scenario serves simultaneously as:

- documentation;
- usage sample;
- regression test;
- proof of functionality;
- executable specification.

Every feature listed below is backed by a real integration test.

---

# Supported Job Types

## 01. Fire-and-Forget

Equivalent to:

```csharp
BackgroundJob.Enqueue(...)
```

Scenario:

```text
Job
 ↓
Execute
 ↓
Completed
```

Verified:

- job is enqueued;
- job executes once;
- job status becomes `Completed`.

---

## 02. Delayed Jobs

Equivalent to:

```csharp
BackgroundJob.Schedule(...)
```

Scenario:

```text
Job
 ↓
Delay
 ↓
Execute
```

Verified:

- job is not executed before scheduled time;
- job becomes eligible after the delay;
- delayed jobs remain pending until execution time.

---

## 03. Recurring Jobs

Available only in the commercial edition.

Equivalent to:

```csharp
RecurringJob.AddOrUpdate(...)
```

Scenario:

```text
CRON
 ↓
Scheduler
 ↓
Jobs Created
 ↓
Execution
```

Verified:

- scheduler creates jobs from CRON expressions;
- recurring schedules can generate multiple executions;
- recurring jobs work independently from workers.

---

## 04. Continuations

Equivalent to:

```text
ActionA
   ↓
ActionB
```

Verified:

- a completed action can enqueue another action;
- continuations execute in the expected order.

---

## 05. Workflows

Scenario:

```text
CreateReport
    ↓
SendReport
    ↓
ArchiveReport
```

Verified:

- multi-step workflows are supported;
- workflow state is preserved through execution;
- actions execute in sequence.

---

## 06. Fan-Out Workflows

Scenario:

```text
Import
 ├─ Validate
 ├─ Notify
 └─ Audit
```

Verified:

- one action can schedule multiple actions;
- parallel workflow branches are supported;
- all child jobs are independently tracked.

---

## 07. Dynamic Routing

Scenario:

```text
Order

Approved
    ↓
Ship

Rejected
    ↓
Cancel
```

Verified:

- workflow paths can be decided at runtime;
- actions can route execution dynamically;
- routing decisions are based on business data.

---

## 08. Retry Policies

Scenario:

```text
Attempt 1 → Failed
Attempt 2 → Failed
Attempt 3 → Completed
```

Verified:

- automatic retries are supported;
- configurable retry delay is supported;
- exponential backoff is supported;
- retry count is tracked.

---

## 09. Queue-Based Workers

Available only in the commercial edition.

Scenario:

```text
emails worker
      ↓
emails queue

sms worker
      ↓
sms queue
```

Equivalent to:

```csharp
await runtime.ExecuteLoopAsync("emails");
```

Verified:

- workers can process a specific queue;
- queues can be isolated;
- different workers can process different workloads.

---

## 10. Progress Reporting

Scenario:

```text
25%  Starting
50%  Working
100% Done
```

Verified:

- actions can report progress;
- progress updates are persisted;
- completion automatically reaches 100%.

---

## 11. Dependency Injection

Scenario:

```text
Service
   ↓
Action Constructor
   ↓
Execution
```

Verified:

- constructor injection is supported;
- registered services are resolved automatically;
- actions can depend on application services.

---

## 12. Failure Handling

Scenario:

```text
Action
 ↓
Exception
 ↓
Failed
```

Verified:

- unhandled exceptions do not crash the worker;
- failed jobs are tracked;
- failure information is persisted.

---

## 13. Failure Continuations

Scenario:

```text
Action
 ↓
Exception
 ↓
OnFailure
 ↓
Compensation
```

Verified:

- compensation actions can be scheduled on failure;
- a failed action does not stop workflow execution;
- failure routing is supported through `JobOptions.OnFailure`.

---

## 14. Execution History

Scenario:

```text
Job
 ↓
Execute
 ↓
Status History
```

Verified:

- job details can be inspected after execution;
- payload and result data are preserved;
- execution state can be queried.

---

## 15. Cancellation

Scenario:

```text
Job
 ↓
Running
 ↓
Cancel
 ↓
Failed (Cancelled)
```

Verified:

- running jobs can be cancelled;
- cancellation is propagated through cancellation tokens;
- worker state is updated correctly.

---

# Summary

## Community Edition

```text
✓ Fire-and-forget jobs
✓ Delayed jobs
✓ Continuation jobs
✓ Multi-step workflows
✓ Fan-out workflows
✓ Dynamic workflow routing
✓ Retry policies
✓ Progress reporting
✓ Dependency injection
✓ Failure handling
✓ Failure continuations
✓ Execution history
✓ Cancellation
```

## Commercial Edition

```text
✓ Recurring CRON jobs
✓ Dedicated queue workers
```

Examples:

```csharp
await runtime.ExecuteLoopAsync("emails");
await runtime.ExecuteOnceAsync("emails");
```

```csharp
RecurringJob.AddOrUpdate(...)
```

---

The IntegrationTests project proves that all listed capabilities work as executable, repeatable scenarios and protects them against future regressions.
