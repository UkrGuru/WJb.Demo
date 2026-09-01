# 📦 WJb Demo (WASM)

Interactive Blazor WebAssembly demo showcasing the WJb background job engine running entirely in the browser.

![WJb Demo (WASM)](https://raw.githubusercontent.com/UkrGuruts/wjb-demo-wasm.png)

🎥 Demo Video:
https://youtu.be/ayTiF1GiwvU

🔗 Repository:
https://github.com/UkrGuru/WJb.Demo

---

## 🧠 What You Will See

```text
Run Action
    ↓
Background Execution
    ↓
Live Progress Updates
    ↓
Workflow Completion
    ↓
Monitor Results
```

This demo allows you to:

- Run jobs directly from the browser
- Observe progress updates in real time
- Explore workflow examples
- Inspect payloads and results
- Review source code behind every sample

👉 No setup required beyond running the application.

---

## ✨ Features

### Dashboard

- ✅ Live job monitoring
- ✅ Progress tracking
- ✅ Job history
- ✅ Failure diagnostics
- ✅ Retry support
- ✅ Payload inspection
- ✅ Result inspection
- ✅ Filtering
- ✅ Paging

### Action Samples

- ✅ Hello Action
- ✅ Configured Action
- ✅ Progress Tracking
- ✅ Ping Website
- ✅ Clean Up Jobs

### Workflow Samples

- ✅ Chained Actions
- ✅ Retry Workflow
- ✅ Order Workflow

### Built-in Source Explorer

Every sample includes source code directly inside the UI.

View:

- Start code
- Payload definitions
- Action implementations
- Workflow logic

---

## 🖥️ Demo Pages

### Hello Action

```text
hello
    ↓
completed
```

### Progress Tracking

```text
0%
25%
50%
75%
100%
```

### Chained Workflow

```text
send-email
      ↓
log
```

### Retry Workflow

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

---

## 💡 What This Demonstrates

- Actions execute in the browser
- Jobs continue independently of UI interactions
- Progress can be reported in real time
- Workflows can schedule additional actions
- Failed jobs can be retried
- Monitoring remains fully transparent

👉 Every workflow step is visible and inspectable.

---

## 🧪 Suggested Scenarios

Try the following:

1. Run Hello Action.
2. Run Progress Tracking and watch updates.
3. Execute a workflow.
4. Open completed jobs.
5. Inspect payloads and results.
6. Trigger a retry scenario.
7. Review the source code for each example.

---

## ▶️ Run Locally

```bash
git clone https://github.com/UkrGuru/WJb.Demo
cd samples/WJb.Demo.Wasm
dotnet run
```

Open:

```text
https://localhost:7268
```

---

## 🔥 Key Idea

WJb workflows are ordinary C# code.

Actions explicitly define what happens next, progress is fully observable, and execution remains transparent from start to finish.

---

## ⚡ Learn More

➡️ https://wjb.pro

➡️ https://www.nuget.org/packages?q=wjb

➡️ https://github.com/UkrGuru/WJb.Demo
