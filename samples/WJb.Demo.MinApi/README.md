# 🚀 WJb Minimal API Demo

A minimal ASP.NET Core API demonstrating background job execution with WJb.

---

## 🧠 What you will see

```text
POST /jobs
    ↓
enqueue
    ↓
background execution
    ↓
progress updates
    ↓
completed job
```

This sample demonstrates:

- job creation through HTTP
- background execution
- progress reporting
- job querying
- job deletion

👉 The API remains responsive while jobs run in the background.

---

## 🚀 Run

```bash
dotnet run
```

Open:

```text
WJb.Demo.MinApi.http
```

and execute the requests directly from Visual Studio.

---

## 🏗 Architecture

```text
POST /jobs
    ↓
IWJb.EnqueueAsync()
    ↓
InMemoryStore
    ↓
WasmWorker
    ↓
DemoAction
    ↓
Completed Job
```

---

## 🔌 API

### Create Job

```http
POST /jobs
```

Response:

```json
{
  "jobId": "019f37e6-a40f-7639-8a24-d77bf860647a"
}
```

### List Jobs

```http
GET /jobs
```

Returns all jobs in the store.

### Get Job

```http
GET /jobs/{id}
```

Returns a single job.

### Delete Job

```http
DELETE /jobs/{id}
```

Removes a job from the store.

---

## ✅ Example Flow

### 1. Create Job

```http
POST /jobs
```

Response:

```json
{
  "jobId": "019f37e6-a40f-7639-8a24-d77bf860647a"
}
```

### 2. Check Progress

```json
{
  "action": "demo",
  "status": 0,
  "progress": 0
}
```

### 3. Check Completed Job

```json
{
  "action": "demo",
  "status": 2,
  "progress": 100,
  "message": "Progress 100%",
  "result": {
    "value": "Done ✅"
  }
}
```

---

## 💡 What this demonstrates

- Minimal API integration
- Background execution
- Progress reporting
- Store-based job management
- Explicit action execution

👉 Jobs are created through HTTP and executed by WJb outside the request pipeline.

---

## 🔥 Key Idea

```csharp
await wjb.EnqueueAsync(DemoAction.Key, payload);
```

👉 HTTP requests enqueue jobs.

The actual work executes later in the background.

---

## ⚡ Learn More

➡️ https://wjb.pro

➡️ https://www.nuget.org/packages?q=wjb

➡️ https://github.com/UkrGuru/WJb.Demo