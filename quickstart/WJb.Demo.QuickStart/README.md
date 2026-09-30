# ⚡ WJb Quick Start

This is the fastest way to understand how WJb works.

---

## 🧠 What you will see

```text
send-email → log-email → done
```

A simple workflow where:

- one job is enqueued
- an action performs work
- the action explicitly schedules the next step
- the workflow completes

👉 No hidden behavior. No magic.

---

## 🚀 Run

```bash
dotnet run
```

---

## ✅ Code

```csharp
// See Program.cs
```

The complete runnable example is available in `Program.cs`.

---

## ✅ Output

```text
=== WJb Quick Start ===

Workflow:
send-email → log-email → done

[App] Enqueue: send-email
[App] Start execution...

[Action] send-email -> user@test.com via smtp.local
[Action] log-email -> Email sent to user@test.com

=== Completed ===
```

---

## 💡 What this demonstrates

- Actions contain business logic
- Actions can use dependency injection
- Actions explicitly define what runs next
- Workflows are deterministic and visible
- Jobs execute through a store-backed runtime

👉 You always know what happens and why.

---

## 🔥 Key Idea

```csharp
return Results.Done()
    .Next("log-email", new
    {
        Message = $"Email sent to {input?.To}"
    });
```

👉 The current action explicitly decides what runs next.

The workflow is ordinary C# code, not hidden framework configuration.

---

## 🧩 ActionResult

Complete the current job:

```csharp
return Results.Done();
```

Complete and store a result:

```csharp
return Results.Done(customer);
```

Complete and schedule another step:

```csharp
return Results.Done()
    .Next("send-email");
```

Schedule multiple steps:

```csharp
return Results.Done()
    .Next("email")
    .Next("audit")
    .Next("metrics");
```

Failures are represented by exceptions:

```csharp
throw new InvalidOperationException(
    "SMTP server unavailable.");
```

---

## 🧠 Mental Model

```text
Action
 ↓
ActionResult
 ↓
Step
```

An action returns:

- a result
- zero or more next workflow steps

Nothing more.

---

## ⚡ Learn More

➡️ https://wjb.pro

➡️ https://www.nuget.org/packages?q=wjb

➡️ https://github.com/UkrGuru/WJb.Demo

---

## 🎁 Support WJb

📧 ukrguru@gmail.com

👉 https://ko-fi.com/ukrguru

---

> Background jobs should be explicit.
>
> If a workflow exists, you should be able to read it.
