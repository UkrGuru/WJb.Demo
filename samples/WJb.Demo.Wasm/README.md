# 📦 WJb.Demo (WASM)

Interactive Blazor WebAssembly demo showcasing the **WJb background job engine** running entirely in the browser.

![WJb.Demo (WASM) ](https://raw.githubusercontent.com/UkrGuru/WJb.Demo/main/assets/wjb-demo-wasm.png)

🔗 GitHub Repository: https://github.com/UkrGuru/WJb.Demo

🎥 Watch the demo:
https://www.youtube.com/watch?v=Tu3TD2Rk37E
***

## 🚀 Overview

**WJb.Demo (WASM)** demonstrates how WJb workflows behave in a real application.

The sample includes runnable action examples, workflow scenarios, progress tracking, retries, and a live monitoring dashboard.

Run an action, switch to the dashboard, and observe execution in real time.

***

## 🧩 Features

### Dashboard

- ✅ Live job monitoring
- ✅ Progress tracking
- ✅ Completed and failed jobs
- ✅ Failure diagnostics
- ✅ Retry support
- ✅ Payload inspection
- ✅ Result inspection
- ✅ Date filtering
- ✅ Paging

### Action Samples

- ✅ Hello Action
- ✅ Configured Action
- ✅ Progress Action
- ✅ Chained Actions
- ✅ Retry Workflow
- ✅ Order Workflow

### Source Explorer

Every sample includes a built-in source viewer:

- ✅ Start code
- ✅ Payload definitions
- ✅ Action implementations
- ✅ Workflow examples

Source files are displayed using a multi-tab viewer similar to modern component documentation sites.

***

## 🖥️ Demo Pages

### Hello Action

A minimal action that immediately completes.

```text
hello
    ↓
completed
```

### Configured Action

Demonstrates dependency injection and service configuration.

```text
smtp settings
      ↓
configured action
```

### Progress Action

Demonstrates progress reporting.

```text
0%
25%
50%
75%
100%
```

### Chained Actions

Demonstrates workflow chaining through `ActionResults.Next(...)`.

```text
send-email
      ↓
log
```

### Retry Workflow

Demonstrates automatic retries using `JobOptions`.

```text
retry-email
      ↓
failure
      ↓
retry
      ↓
success
      ↓
log
```

### Order Workflow

Demonstrates a complete multi-step workflow.

```text
create-order
      ↓
reserve-stock
      ↓
charge-payment
      ↓
send-confirmation
      ↓
log
```

***

## ⚙️ How It Works

WJb separates responsibilities into:

- **Actions** - business logic
- **Store** - job persistence
- **Executor** - job execution
- **Worker** - background processing
- **Monitor** - execution visibility

In this demo:

- Jobs are started from the UI
- Actions execute inside the WASM runtime
- Progress updates appear immediately
- Workflows can enqueue additional actions
- Failed jobs can be retried directly from the dashboard

***

## 🧪 Scenarios

Try the following:

- Run a simple Hello Action
- Execute a configured SMTP action
- Watch progress updates in real time
- Trigger a chained workflow
- Observe failure handling
- Retry a failed action
- Execute a complete order workflow

***

## ▶️ Run Locally

```bash
git clone https://github.com/UkrGuru/WJb.Demo
cd WJb.Demo
dotnet run
```

Open:

```text
https://localhost:7268
```

***

## 📌 Notes

- Runs entirely in Blazor WebAssembly.
- Intended for learning and experimentation.
- Demonstrates core WJb workflow capabilities.
- Source examples are included directly inside the demo UI.
- Additional scenarios may be added over time.

***
