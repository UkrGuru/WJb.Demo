# 🚀 WJb Minimal API Demo

A minimal ASP.NET Core API using **WJb** to execute background jobs.

This sample demonstrates:

- Job enqueueing via HTTP
- Background job processing
- Progress reporting
- In-memory storage
- Job monitoring endpoints

---

## 🧠 Architecture

```text
POST /jobs
     │
     ▼
 Enqueue "demo"
     │
     ▼
 InMemoryStore
     │
     ▼
 JobWorker
     │
     ▼
 DemoAction
     │
     ▼
 Progress 0% → 100%
     │
     ▼
 Result
```

---

## ⚡ Features

✅ ASP.NET Core Minimal API

✅ Background worker

✅ In-memory job store

✅ Progress notifications

✅ Job monitoring endpoints

✅ Explicit action registration

---

## 📦 Action Registration

```csharp
cfg.AddAction<DemoAction>(DemoAction.Key);
```

Action key:

```csharp
public const string Key = "demo";
```

---

## 🚀 Run

```bash
dotnet run
```

Example:

```text
http://localhost:5102
```

---

# API

## Create Job

Creates a new background job.

### Request

```http
POST /jobs
```

### Response

```json
{
  "jobId": "019f37e6-a40f-7639-8a24-d77bf860647a"
}
```

---

## List Jobs

Returns all jobs.

### Request

```http
GET /jobs
```

### Response

```json
[
  {
    "id": "019f37e6-a40f-7639-8a24-d77bf860647a",
    "action": "demo",
    "status": 0,
    "createdAt": "2026-07-06T14:48:09.9993968Z",
    "progress": 0,
    "message": null
  }
]
```

---

## Get Job

Returns a single job.

### Request

```http
GET /jobs/{id}
```

Example:

```http
GET /jobs/019f37e6-a40f-7639-8a24-d77bf860647a
```

---

## Delete Job

Removes a job from storage.

### Request

```http
DELETE /jobs/{id}
```

---

# Demo Action

The sample action simulates long-running work.

```csharp
public sealed class DemoAction
    : JobAction<DemoPayload>,
      IProgressAction
{
    public const string Key = "demo";

    public override async Task<ActionResult> ExecuteAsync(
        DemoPayload input,
        CancellationToken ct)
    {
        for (var i = 0; i <= 100; i += 10)
        {
            await Task.Delay(input.DelayMs / 10, ct);

            OnProgress?.Invoke(new JobProgress
            {
                Progress = i,
                Message = $"Progress {i}%"
            });
        }

        return ActionResults.Result(new
        {
            ok = true,
            text = input.Text
        });
    }
}
```

---

## Progress Flow

```text
0%
10%
20%
30%
40%
50%
60%
70%
80%
90%
100%
```

---

## Payload

```csharp
public sealed class DemoPayload
{
    public int DelayMs { get; set; }

    public string Text { get; set; } = "";
}
```

Example payload:

```json
{
  "delayMs": 5000,
  "text": "Done ✅"
}
```

---

# Worker

Jobs are processed continuously by a background service.

```csharp
public sealed class JobWorker : BackgroundService
{
    protected override Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        return executor.ExecuteLoopAsync(stoppingToken);
    }
}
```

---

# What This Demonstrates

- WJb integrates naturally with ASP.NET Core
- Actions contain business logic
- Job execution happens in the background
- Progress can be reported during execution
- Jobs can be queried through HTTP endpoints
- No hosted workflow engine is required

---

# Next Steps

Try:

- Email sending
- File processing
- Scheduled jobs
- Workflow chaining with `JobCommand`
- Persistent stores (SQL Server, PostgreSQL, Redis)

---

## Learn More

➡️ https://www.nuget.org/packages?q=WJb

➡️ https://github.com/UkrGuru/WJb.Demo

---

## Support WJb

If you like this project:

👉 https://ko-fi.com/ukrguru

Early supporters (before August 1, 2026):

👉 🎁 **FREE Solo License**
```
