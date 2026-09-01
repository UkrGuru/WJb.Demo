# ⚡ WJb Quick Start

This is the fastest way to understand how WJb works.

---

## 🧠 What you will see

```text
send-email → log → done
```

A simple workflow where:

- one job is enqueued
- an action performs work
- the action explicitly decides what runs next
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
send-email → log → done

[App] Enqueue: send-email
[App] Start execution...

[Action] send-email -> user@test.com via smtp.local
[Action] log -> Email sent to user@test.com

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
return NextAsync<LogAction>(
    new LogInput
    {
        Message = $"Email sent to {input.To}"
    });
```

👉 The current action explicitly decides what runs next.

The workflow is ordinary C# code, not hidden framework configuration.

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