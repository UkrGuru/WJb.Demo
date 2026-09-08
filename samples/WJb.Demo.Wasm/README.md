# 📦 WJb Demo

Interactive demo applications showcasing the WJb background job engine in multiple environments.

---

> [!NOTE]
> The solution contains two Blazor WebAssembly demo applications.
>
> - `WJb.Demo.Wasm.InMemory` is available to all users.
> - `WJb.Demo.Wasm.IndexedDB` depends on `WJb.IndexedDB`.
>
> `WJb.IndexedDB` is available only in the commercial edition.
>
> Learn more: https://wjb.pro/pricing

---

## 🏗 Solution Structure

```text
WJb.Demo.Wasm.InMemory
    ├─ Runs entirely in browser memory
    └─ No persistence between page refreshes

WJb.Demo.Wasm.IndexedDB
    ├─ Uses IndexedDB storage
    ├─ Persists jobs and definitions locally
    └─ Requires WJb.IndexedDB (commercial edition)

WJbPro.Demos
    ├─ Shared actions
    ├─ Shared workflows
    ├─ Shared UI pages
    ├─ Demo resources
    └─ JSON definitions
```

The demo content is shared between both applications, allowing the same actions, workflows, pages, and examples to run against different storage providers.

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

The demo allows you to:

- Run jobs directly from the browser
- Observe progress updates in real time
- Explore workflow examples
- Inspect payloads and results
- Review source code behind every sample
- Compare different storage providers

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

- Action definitions
- Service definitions
- Payload models
- Action implementations
- Workflow logic
- Startup configuration

---

## 📄 JSON-Based Definitions

Demo actions and services are defined in JSON files:

```text
App_Data/actions.json
App_Data/services.json
```

At startup the demo can automatically load or refresh definitions in the selected store.

This makes it easy to:

- Add new examples
- Modify demo workflows
- Experiment with configuration
- Keep demo content synchronized

---

## 🖥 Demo Hosts

### WASM InMemory

```text
Storage: InMemory
Persistence: No
License: Included
```

Runs entirely in browser memory.

Refreshing the page starts with a clean store.

### WASM IndexedDB

```text
Storage: IndexedDB
Persistence: Yes
License: Commercial Edition
```

Uses the `WJb.IndexedDB` package.

Jobs, definitions, and configuration remain available after page refreshes.

> `WJb.IndexedDB` is available only in the commercial edition.
>
> Learn more: https://wjb.pro/pricing

---

## 💡 What This Demonstrates

- Background jobs running inside Blazor WebAssembly
- Multiple storage implementations
- Action execution
- Workflow orchestration
- Real-time progress reporting
- Retry scenarios
- Transparent monitoring
- Source-driven configuration

Every workflow step is visible and fully inspectable.

---

## 🧪 Suggested Scenarios

1. Run Hello Action.
2. Execute Progress Tracking.
3. Open the Jobs dashboard.
4. Review live progress updates.
5. Explore workflow examples.
6. Trigger a retry scenario.
7. Inspect payloads and results.
8. Compare InMemory and IndexedDB behavior.
9. Review the JSON definitions.
10. Explore the source code behind each example.

---

## ▶️ Run Locally

```bash
git clone https://github.com/UkrGuru/WJb.Demo
```

### Run InMemory Demo

```bash
cd WJb.Demo.Wasm.InMemory
dotnet run
```

### Run IndexedDB Demo

```bash
cd WJb.Demo.Wasm.IndexedDB
dotnet run
```

> [!IMPORTANT]
> The IndexedDB demo requires the `WJb.IndexedDB` package.
>
> Available only in the commercial edition:
> https://wjb.pro/pricing

---

## 🔥 Key Idea

WJb workflows are ordinary C# code.

Actions explicitly determine what happens next, progress is fully observable, and execution remains transparent from start to finish.

The demo applications showcase the same workflows running against different storage providers using a shared set of actions, workflows, pages, and JSON definitions.

---

## ⚡ Learn More

➡️ https://wjb.pro

➡️ https://wjb.pro/pricing

➡️ https://www.nuget.org/packages?q=wjb

➡️ https://github.com/UkrGuru/WJb.Demo
