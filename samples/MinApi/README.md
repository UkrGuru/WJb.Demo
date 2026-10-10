# 🚀 WJb Minimal API Demo

A minimal ASP.NET Core API demonstrating background job execution with **WJb**.

---

## 🧠 What you will see

```text
POST /jobs
    ↓
enqueue
    ↓
background execution (WasmWorker)
    ↓
DemoAction (with progress)
    ↓
completed job
```

This sample demonstrates:

- Job creation through HTTP
- Background execution outside the request pipeline
- Action execution with progress reporting
- Job querying and deletion
- Responsive API while jobs run in the background

---

## 🚀 Run

```bash
dotnet run
```

Open `MinApi.http` and execute the requests directly from Visual Studio / Rider / VS Code.

---

## 🏗 Architecture

```text
POST /jobs
    ↓
IStore.EnqueueAsync()
    ↓
InMemoryStore
    ↓
WasmWorker
    ↓
DemoAction (IProgressAction)
    ↓
ActionResult
    ↓
Completed Job
```

---

## 🔌 API

### Create Job

```http
POST /jobs
```

Enqueues a demo job with:

```csharp
new DemoPayload { DelayMs = 5000, Text = "Done ✅" }
```

**Response:**

```json
{
  "jobId": "019f37e6-a40f-7639-8a24-d77bf860647a"
}
```

### List Jobs

```http
GET /jobs
```

Returns all jobs currently in the store.

### Get Job

```http
GET /jobs/{id}
```

Returns a single job (including status, progress and result).

### Delete Job

```http
DELETE /jobs/{id}
```

Removes a job from the store.

---

## ✅ Example Flow

### 1. Create a job

```http
POST /jobs
```

### 2. DemoAction runs in the background

```csharp
public override async ValueTask<ActionResult> ExecuteAsync(
    DemoPayload? input, CancellationToken ct = default)
{
    var delayMs = input?.DelayMs ?? 0;

    for (var i = 0; i <= 100; i += 10)
    {
        ct.ThrowIfCancellationRequested();

        await Task.Delay(delayMs / 10, ct);

        ReportProgress(i, $"Progress {i}%");
    }

    return new ActionResult(input?.Text);
}
```

### 3. Completed job looks like

```json
{
  "action": "demo",
  "status": "completed",
  "result": {
    "value": "Done ✅"
  }
}
```

While the job is running you can observe progress updates (`0%` → `100%`).

---

## 💡 What this demonstrates

- Minimal API integration
- Background execution via `WasmWorker`
- `InMemoryStore` for job management
- Progress reporting (`IProgressAction`)
- Clean separation between HTTP and job execution

👉 Jobs are created through HTTP and executed by WJb **outside** the request pipeline.

---

## 🔥 Key Idea

```csharp
var jobId = await store.EnqueueAsync(DemoAction.Key,
    new DemoPayload { DelayMs = 5000, Text = "Done ✅" });
```

HTTP requests only enqueue jobs.

The real work happens in the action:

```csharp
return new ActionResult(input?.Text);
```

Everything is ordinary, explicit C# code.

---

## 🧩 ActionResult

```csharp
return ActionResult.Empty;                    // empty result
return new ActionResult(someValue);           // with result value
```

Failures are represented by exceptions:

```csharp
throw new InvalidOperationException("Something failed.");
```

---

## 🧠 Mental Model

```text
HTTP Request
    ↓
Enqueue
    ↓
Store
    ↓
Worker
    ↓
Action (+ Progress)
    ↓
ActionResult
    ↓
Completed Job
```

Everything is explicit and observable.

---

## ⚡ Learn More

➡️ https://wjb.pro  
➡️ https://www.nuget.org/packages?q=wjb  
➡️ https://github.com/UkrGuru/WJb.Demo
