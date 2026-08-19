# 🗄️ WJb.Sql

WJb.Sql is the SQL Server storage provider for WJb.

> // Available only in the commercial edition.

```text
WJb
 ↓
WJb.Sql
 ↓
SQL Server
```

It provides durable storage for jobs, payloads, results, progress updates, definitions, and workflow history.

---

## Why WJb.Sql

The in-memory store is ideal for development.

Production systems need durability.

```text
Process Restart
        ↓
Server Reboot
        ↓
Deployment
        ↓
WJb.Sql
        ↓
Nothing Lost
```

WJb.Sql persists:

- Jobs
- Payloads
- Results
- Progress updates
- Action definitions
- Service definitions
- Workflow history

---

## Installation

```bash
dotnet add package WJb.Sql
```

---

## Creating a Store

```csharp
var store =
    new SqlStore(
        () => new SqlConnection(connectionString));
```

The connection factory is used whenever database access is required.

---

## Basic Usage

```csharp
var store =
    new SqlStore(
        () => new SqlConnection(connectionString));

var wjb =
    await WJbBuilder.CreateAsync(store);
```

Once the instance is created, WJb is used exactly the same way as with the in-memory store.

---

## Scheduled Jobs

Delayed jobs are stored immediately.

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload,
    new JobOptions
    {
        Delay = TimeSpan.FromHours(1)
    });
```

The SQL store prevents execution until the scheduled time.

---

## Queues

Jobs can be routed to queues.

```csharp
await wjb.EnqueueAsync(
    "send-email",
    payload,
    new JobOptions
    {
        Queue = "email"
    });
```

Worker:

```csharp
await wjb.ExecuteLoopAsync(
    queue: "email");
```

---

## Progress Tracking

Actions may report progress.

```csharp
ReportProgress(
    50,
    "Processing records");
```

Progress is persisted in SQL Server.

---

## Results

Job results are stored as JSON.

```json
123
```

```json
"Done"
```

```json
{
  "sent": true
}
```

---

## Errors

Failure information is stored and preserved.

```text
SMTP unavailable
```

```text
Validation failed
```

---

## Monitoring

The SQL store keeps workflow history and job execution information.

Useful for:

- Monitoring
- Diagnostics
- Reporting
- Administration

---

## Scaling

Typical deployment:

```text
Application
      ↓
  WJb.Sql
      ↓
 SQL Server
```

Multiple workers can share the same database.

```text
Worker 1
Worker 2
Worker 3
     ↓
SQL Server
```

---

## Best Practices

✅ Use SQL Server backups

✅ Keep payloads reasonably small

✅ Store large content separately

✅ Monitor failed jobs

✅ Use queues for workload isolation

❌ Store large files inside jobs

❌ Put business logic into SQL

❌ Use SQL as file storage

---

## Commercial Edition

WJb.Sql is a commercial package.

The WJb runtime remains independent of SQL Server.

---

## Mental Model

```text
WJb         = Job Engine

Action      = Business Logic

WJb.Sql     = Persistence

SQL Server  = Storage
```

That's it.

---

## Source Code

Documentation examples are verified by automated documentation tests.

Tests:

```text
../test/WJb.DocTests/12_WJbSqlTests.cs
```

---

> WJb defines execution flow.
>
> WJb.Sql makes it durable.
