# WJb Documentation

WJb is an explicit background job engine for .NET where workflow transitions are defined in code and never hidden behind pipelines or middleware.

```text
Job
 ↓
Action
 ↓
ActionResult
 ↓
Step
 ↓
Next Job
```

Every workflow is visible.  
Every transition is explicit.  
Every step is defined in code.

---

## Getting Started

Start here if you are new to WJb.

### Core Concepts

- [Actions](actions.md)
- [ActionResult](action-result.md)
- [Step](step.md)
- [JobOptions](job-options.md)

---

## Execution

Learn how jobs run.

- [Executor](executor.md)
- [Progress](progress.md)
- [Retry](retry.md)
- [Scheduling](scheduling.md)
- [Queues](queues.md)

---

## Storage

Learn how jobs are persisted.

- [Store](store.md)
- [Custom Stores](custom-stores.md)

---

## Packages

### Core

- **WJb**  
  Explicit background job engine.

### Commercial

- **WJb.SqlServer**
- **WJb.PostgreSql**
- **WJb.MySql**
- **WJb.Sqlite**
- **WJb.IndexedDB**
- **WJb.Pro**

### UI

- [WJb.UI.Blazor](wjb-ui-blazor.md)

---

## Philosophy

Many background job systems evolve into:

```text
Job
 ↓
Retry
 ↓
Pipeline
 ↓
Middleware
 ↓
???
```

WJb intentionally keeps workflows explicit.

```text
Action
 ↓
ActionResult
 ↓
Step
 ↓
Next Job
```

If you can answer:

- Why did this job run?
- What did it do?
- What runs next?
- Why was it retried?

by reading the code, the workflow is explicit.

That is the core idea behind WJb.

---

## FAQ

Common questions:

- [FAQ](faq.md)

---

## Source Code

Documentation examples are verified by automated documentation tests.

Tests:

```text
../test/WJb.DocTests
```

---

## Support

📧 ukrguru@gmail.com  

☕ https://ko-fi.com/ukrguru

---

> Background jobs shouldn't be magic.
