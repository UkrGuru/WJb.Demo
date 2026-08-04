# WJb.UI.Blazor

WJb.UI.Blazor provides monitoring and administration tools for WJb.

```text
Workers
    ↓
 WJb Store
    ↓
WJb.UI.Blazor
    ↓
 Browser
```

The UI helps you understand what is happening inside your job system.

---

## What It Provides

Typical capabilities:

✅ Job monitoring

✅ Job history

✅ Progress tracking

✅ Status filtering

✅ Job details

✅ Job deletion

✅ Queue visibility

✅ Error inspection

---

## Installation

```bash
dotnet add package WJb.UI.Blazor
```

---

## Basic Setup

Register the UI services.

```csharp
builder.Services.AddWJbUi();
```

Map the UI endpoint.

```csharp
app.MapWJbUi();
```

---

## Dashboard

The dashboard provides a high-level overview of the system.

Example:

```text
Pending      25

Running       3

Completed  1,253

Failed        7
```

This helps identify issues quickly.

---

## Job List

Browse jobs using filters.

Example:

```text
All Jobs

Pending

Running

Completed

Failed
```

The UI can display jobs from any supported store.

---

## Job Details

Select a job to inspect its details.

Typical information:

```text
Id

Action

Status

Queue

RunAt

CreatedAt

UpdatedAt
```

---

## Payload Viewer

Inspect the original job payload.

Example:

```json
{
    "to": "user@test.com",
    "subject": "Monthly Report"
}
```

Useful for troubleshooting.

---

## Result Viewer

Successful jobs can store a result.

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

The UI displays the stored JSON.

---

## Error Viewer

Failed jobs include failure information.

Examples:

```json
"SMTP unavailable"
```

```json
{
    "message": "SMTP unavailable"
}
```

This helps diagnose problems quickly.

---

## Progress Monitoring

Actions can report progress.

```csharp
await Context.UpdateProgressAsync(
    50,
    "Processing records");
```

Displayed as:

```text
Progress: 50%

Processing records
```

---

## Queue Visibility

The UI displays queue information.

Example:

```text
email

reports

imports

default
```

This helps verify workload routing.

---

## Searching Jobs

Search by:

```text
Id

Action

Queue

Status
```

Quick access reduces troubleshooting time.

---

## Pagination

Large job histories are paged automatically.

```text
Page 1

Page 2

Page 3
```

This keeps the UI responsive.

---

## Administration

Administrative operations may include:

```text
Delete Job

Inspect Payload

Inspect Result

Inspect Error
```

Available operations may vary by store implementation.

---

## Production Use

Typical deployment:

```text
Application
      ↓
 WJb Workers
      ↓
   WJb.Sql
      ↓
 SQL Server
      ↓
WJb.UI.Blazor
```

The UI can be hosted in the same application or separately.

---

## Best Practices

✅ Restrict access

✅ Monitor failed jobs

✅ Review queue health

✅ Track long-running jobs

✅ Monitor progress updates

❌ Expose the dashboard publicly

❌ Ignore failed jobs

❌ Store sensitive secrets inside payloads

---

## Mental Model

```text
Executor = Runs Jobs

Store    = Saves Jobs

UI       = Explains Jobs
```

The purpose of WJb.UI.Blazor is simple:

> Show what happened, what is happening, and what will happen next.