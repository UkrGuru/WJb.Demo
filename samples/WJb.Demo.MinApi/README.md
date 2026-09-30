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
action execution
    ↓
completed job
```

This sample demonstrates:

- job creation through HTTP
- background execution
- action execution
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
Store
    ↓
Worker
    ↓
Action
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

### 2. Execute Action

```csharp
return Results.Done()
    .Next("send-email");
```

### 3. Check Completed Job

```json
{
  "action": "demo",
  "status": "completed",
  "result": {
    "value": "Done ✅"
  }
}
```

---

## 💡 What this demonstrates

- Minimal API integration
- Background execution
- Store-based job management
- Explicit action execution
- Explicit workflow transitions

👉 Jobs are created through HTTP and executed by WJb outside the request pipeline.

---

## 🔥 Key Idea

```csharp
await wjb.EnqueueAsync("demo", payload);
```

HTTP requests enqueue jobs.

```csharp
return Results.Done()
    .Next("process-order");
```

Actions explicitly decide what runs next.

The workflow is ordinary C# code, not hidden framework configuration.

---

## 🧩 ActionResult

```csharp
return Results.Done();
```

```csharp
return Results.Done(customer);
```

```csharp
return Results.Done()
    .Next("send-email");
```

```csharp
return Results.Done()
    .Next("email")
    .Next("audit");
```

Failures are represented by exceptions:

```csharp
throw new InvalidOperationException("Something failed.");
```

---

## 🧠 Mental Model

```text
HTTP
 ↓
Action
 ↓
ActionResult
 ↓
Step
```

Everything is explicit.

---

## ⚡ Learn More

➡️ https://wjb.pro

➡️ https://www.nuget.org/packages?q=wjb

➡️ https://github.com/UkrGuru/WJb.Demo
