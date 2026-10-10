# 📊 WJb Monitor Demo

A complete workflow monitoring sample built with WJb.

Run a workflow, observe execution in real time, inspect payloads, review results, and explore registered actions and services.

![WJb Monitor](https://raw.githubusercontent.com/UkrGuru/WJb.Demo/main/assets/wjb-monitor.png)

---

## ✅ Supported Storage Providers

### Included

- InMemoryStore

### Commercial Edition

SQL providers:

- WJb.SqlServer
- WJb.PostgreSql
- WJb.Sqlite
- WJb.MySql

Additional providers:

- WJb.IndexedDB

All providers expose the same WJb APIs and monitoring experience.

---

## 🧠 What You Will See

```text
import-customers
        ↓
generate-report
        ↓
send-email
```

Every step is visible in the monitoring dashboard.

The sample demonstrates:

- Workflow execution
- Progress reporting
- Action chaining
- Retry handling
- Job inspection
- Action discovery
- Service discovery

👉 Follow a workflow from the first job to the final result.

---

## 📦 Available Monitor Samples

The solution includes dedicated monitor applications for different storage providers:

```text
WJb.Demo.Monitor.InMemory
WJb.Demo.Monitor.SqlServer
WJb.Demo.Monitor.PostgreSql
WJb.Demo.Monitor.Sqlite
WJb.Demo.Monitor.MySql
```

Each sample demonstrates the same workflow and monitoring experience.

Only the storage provider changes.

---

## 🚀 Run

```bash
dotnet run
```

Open:

```text
https://localhost:7077
```

---

## 🏗 Storage

The default sample uses an in-memory store:

```csharp
var store = new InMemoryStore();
```

No database setup is required.

SQL and IndexedDB providers are also available for production scenarios.

> **Available only in the commercial edition.**

---

## ⚙️ Workflow Setup

Actions and services are registered programmatically:

```csharp
await store.AddActionAsync<ImportCustomersAction>(Actions.ImportCustomers);
await store.AddActionAsync<GenerateReportAction>(Actions.GenerateReport);
await store.AddActionAsync<SendEmailAction>(Actions.SendEmail);

await store.AddServiceAsync(new SmtpSettings
{
    Host = "smtp.demo.local",
    Port = 25,
    From = "noreply@demo.local"
});
```

The **Actions** and **Services** pages allow you to inspect these registrations in real time.

---

## 🏗 Workflow

Start the workflow:

```csharp
await wjb.EnqueueAsync(
    Actions.ImportCustomers,
    new ImportCustomersInput
    {
        Source = "CRM"
    });
```

Execution flow:

```text
import-customers
        ↓
generate-report
        ↓
send-email
```

Each action explicitly decides what happens next.

---

## 👀 Monitor Features

### Jobs

View and inspect:

- Status
- Progress
- Payloads
- Results
- Execution history
- Failures
- Retry information

Additional features:

- Date filtering
- Paging
- Job details

### Actions

Explore registered actions:

- Action metadata
- Action definitions
- Configuration editing

### Services

Explore registered services:

- Service configuration
- Service metadata
- Runtime values

---

## 💡 What This Demonstrates

- Typed action inputs
- Dependency injection
- Explicit workflow transitions
- Progress reporting
- Background execution
- Job monitoring
- Failure diagnostics
- Retry workflows

👉 Monitoring is built around real workflow execution, not simulated data.

---

## 🔥 Key Idea

```text
Action
   ↓
Job
   ↓
Monitor
```

WJb keeps workflow execution explicit while providing complete visibility into what happened, when it happened, and why.

No workflow designer.

No XML.

No hidden execution flow.

Just ordinary C# actions connected through explicit transitions.

---

## 🧪 Suggested Scenarios

Try the following:

1. Run the demo workflow.
2. Open the generated jobs.
3. Inspect payloads.
4. Review action results.
5. Force a failure.
6. Retry the failed job.
7. Explore registered actions.
8. Explore registered services.

---

## 🗄 Commercial Storage Providers

WJb offers multiple production-ready storage providers.

### SQL Providers

```text
WJb.SqlServer
WJb.PostgreSql
WJb.Sqlite
WJb.MySql
```

### Browser Storage

```text
WJb.IndexedDB
```

All providers use the same programming model and monitoring UI.

Switching providers typically requires only store registration changes.

> **Available only in the commercial edition.**

---

## 💼 Commercial Features

### Storage Providers

Included in the commercial edition:

- WJb.SqlServer
- WJb.PostgreSql
- WJb.Sqlite
- WJb.MySql
- WJb.IndexedDB

Benefits:

- Persistent storage
- Monitoring history
- Job retention
- Production deployment support
- Identical WJb APIs
- Identical monitoring experience

Learn more:

https://wjb.pro/pricing

---

## ⚡ Learn More

➡️ https://wjb.pro

➡️ https://www.nuget.org/packages?q=wjb

➡️ https://github.com/UkrGuru/WJb.Demo