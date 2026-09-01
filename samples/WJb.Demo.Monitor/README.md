# 📊 WJb Monitor Demo

A complete workflow monitoring sample built with WJb.

Run a workflow, observe execution in real time, inspect payloads, review results, and explore registered actions and services.

![WJb Monitor](https://raw.githubusercontent.com/UkrGuru/WJb.Demo/main/assets/wjb-monitor.png)

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

The demo uses an in-memory store by default:

```csharp
var store = new InMemoryStore();
```

No database setup is required.

For production environments, WJb also provides a SQL Server store implementation.

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
await wjb.EnqueueAsync(Actions.ImportCustomers,
    new ImportCustomersInput { Source = "CRM" });
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

## 🗄 Using SqlStore

The demo uses `InMemoryStore` by default and runs without any database setup.

To use `SqlStore`, make the following changes.

### 1. Enable the package reference

In the project file:

```xml
<ItemGroup>
  <!-- <PackageReference Include="WJb.Sql" Version="0.118.0" /> -->
  <PackageReference Include="WJb.UI.Blazor" Version="0.118.0" />
</ItemGroup>
```

Uncomment:

```xml
<PackageReference Include="WJb.Sql" Version="0.117.2-beta.1" />
```

### 2. Switch the store implementation

In `Program.cs` replace:

```csharp
// use InMemoryStore for testing purposes
var store = new InMemoryStore();
```

with:

```csharp
using Microsoft.Data.SqlClient;
using WJb.Sql;

const string connectionString =
    "Server=(localdb)\\MSSQLLocalDB;Database=WJbMonitor;Trusted_Connection=True;TrustServerCertificate=True;";

await using (var conn = new SqlConnection(connectionString))
{
    await conn.InitDbAsync();
}

var store = new SqlStore(() => new SqlConnection(connectionString));
```

The rest of the application remains unchanged.

> **SqlStore is available only in the commercial edition.**

---

## 💼 Commercial Features

### SqlStore

WJb includes a SQL Server backed store implementation:

```csharp
using WJb.Sql;
```

This package is available only in the commercial edition.

Learn more at:

https://wjb.pro/pricing

---

## ⚡ Learn More

➡️ https://wjb.pro

➡️ https://www.nuget.org/packages?q=wjb

➡️ https://github.com/UkrGuru/WJb.Demo