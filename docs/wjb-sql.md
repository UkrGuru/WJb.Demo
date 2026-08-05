# WJb.Sql

WJb.Sql is the SQL Server storage provider for WJb.

> // Available only in the commercial edition.

```text
WJb
 ↓
WJb.Sql
 ↓
SQL Server
```

It provides durable job storage, scheduling, progress tracking, and job history.

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

## Creating Tables

Run the provided SQL installation script.

```text
WJb_Jobs
```

The table stores:

- Jobs
- Status
- Payload
- Results
- Errors
- Progress
- Scheduling information

---

## Basic Usage

```csharp
var store =
    new SqlStore(
        () => new SqlConnection(connectionString));

var wjb = WJbBuilder.Create(store);
```

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
await Context.UpdateProgressAsync(
    50,
    "Processing records");
```

Progress is persisted in SQL Server.

---

## Results

Job results are stored as JSON.

Examples:

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

Failure information is stored as JSON.

Examples:

```json
"SMTP unavailable"
```

```json
{
    "message": "SMTP unavailable"
}
```

---

## Monitoring

The SQL store keeps historical job information.

Useful for:

- Monitoring
- Diagnostics
- Reporting
- Administration

---

## Production Usage

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

The core WJb package remains independent of SQL Server.

---

## Mental Model

```text
WJb     = Execution

Action  = Business Logic

WJb.Sql = Persistence
```

WJb.Sql is responsible for storing jobs.

Nothing else changes.

---

## Source Code

Documentation examples are verified by automated documentation tests.

Tests:

-[../test/WJb.DocTests/12 WjbSqlTests.cs](../test/WJb.DocTests/12%20WjbSqlTests.cs)