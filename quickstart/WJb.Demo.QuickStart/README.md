# ⚡ WJb QuickStart

This is the fastest way to understand how WJb works.

---

## 🧠 What you will see

```

SendEmail → Log → Done

```

A simple job flow where:

- one job runs  
- then explicitly enqueues the next  
- and completes  

👉 no hidden behavior

---

## 🚀 Run

```bash
dotnet run
````

***

## ✅ Output

```
=== WJb QuickStart ===

Flow:
SendEmail → Log → Done

[App] Enqueue: SendEmail
[App] Start execution...

[Action] SendEmail → user@test.com
[Action] Log → Email sent to user@test.com

=== Done ===
All steps were explicitly defined.
```

***

## 💡 What this demonstrates

* Actions contain business logic
* Each action **explicitly defines next step**
* Execution is **deterministic and visible**

👉 You always know what happens and why

***

## 🔥 Key idea

```csharp
return ActionResults.Next(
    JobCommands.Next<LogAction>(...)
);
```

👉 The workflow is defined in code, not hidden in the framework.

***

## ⚡ Learn more

➡️ <https://www.nuget.org/packages/WJb>  
➡️ <https://github.com/UkrGuru/WJb.Demo>

***

## 🎁 Support WJb

If you like this idea:

👉 <https://ko-fi.com/ukrguru>

Early supporters (before August 1, 2026):

👉 🎁 **FREE Solo License**

