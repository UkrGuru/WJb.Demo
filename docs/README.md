# WJb Documentation

WJb is an explicit background job engine for .NET.

```text
Job
 ↓
Action
 ↓
ActionResult
 ↓
JobCommand
```

Every workflow is visible.

Every transition is explicit.

Every step is defined in code.

---

## Getting Started

Start here if you are new to WJb.

### Core Concepts

- actions.md
- action-result.md
- [JobCommand](job-command.md)
- [JobOptions](job-options.md)

---

## Execution

Learn how jobs run.

- Executor
- progress.md
- [etry
- scheduling.md
- queues.md

---

## Storage

Learn how jobs are persisted.

- store.md
- [Custom Stores](custom
## Packages

### Core

- **WJb**

Explicit background job engine.

### Commercial

- **WJb.Sql**
- **WJb.Pro**

Documentation:

- [WJbsql.md

### UI

- [WJ-ui-blazor.md

---

## Common Reading Paths

### I Want To Learn WJb

Read in this order:

1. [Actionsmd
2. [ActionResult](action-result.nd.md
4. [JobOptions.md
5. [Executor](executor## I Want Production Deployment

Read:

1. store.md
2. [Queues3. scheduling.md
4. retry.md
5. [WJbsql.md

---

### I Want To Build Extensions

Read:

1. store.md
2. [Custom Stores](customtor.md

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

- faq.md

---

## Support

📧 ukrguru@gmail.com

☕ https://ko-fi.com/ukrguru

---

> Background jobs shouldn't be magic.