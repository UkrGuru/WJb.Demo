# WJb Documentation

WJb is an explicit background job engine for .NET where workflow transitions are defined in code and never hidden behind pipelines or middleware.
```text
Job
 ↓
Action
 ↓
ActionResult
 ↓
JobCommand
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
- [JobCommand](job-command.md)
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

- **WJb.Sql**
- **WJb.Pro** (coming soon)

Documentation:

- [WJb.Sql](wjb-sql.md)

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
JobCommand
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

## Support

📧 ukrguru@gmail.com

☕ https://ko-fi.com/ukrguru

---

> Background jobs shouldn't be magic.