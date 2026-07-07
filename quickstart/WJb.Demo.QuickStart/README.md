# ⚡ WJb Quick Start

This is the fastest way to understand how WJb works.

---

## 🧠 What you will see

```text
send-email → log → done
```

A simple workflow where:

* one action runs
* explicitly enqueues the next action
* and then completes

👉 No hidden behavior. No magic.

***

## 🚀 Run

```bash
dotnet run
```

***

## ✅ Output

```text
=== WJb Quick Start ===

Workflow:
send-email → log → done

[App] Enqueue: send-email
[App] Start execution...

[Action] send-email → user@test.com
[Action] log → Email sent to user@test.com

=== Completed ===
```

***

## 💡 What this demonstrates

* Actions contain business logic
* Each action explicitly defines the next step
* Workflow transitions are visible in code
* Execution is deterministic and easy to reason about

👉 You always know what happens and why.

***

## 🔥 Key idea

```csharp
return ActionResults.Next(
    new JobCommand(
        LogAction.Key,
        new LogInput
        {
            Message = $"Email sent to {input.To}"
        })
);
```

👉 The workflow is defined in your code, not hidden inside the framework.

***

## 📄 Full Example

```csharp
public sealed class SendEmailAction : JobAction<EmailInput>
{
    public const string Key = "send-email";

    public override Task<ActionResult> ExecuteAsync(
        EmailInput input,
        CancellationToken ct)
    {
        Console.WriteLine($"[Action] {Key} → {input.To}");

        return Task.FromResult(
            ActionResults.Next(
                new JobCommand(
                    LogAction.Key,
                    new LogInput
                    {
                        Message = $"Email sent to {input.To}"
                    })
            )
        );
    }
}

public sealed class LogAction : JobAction<LogInput>
{
    public const string Key = "log";

    public override Task<ActionResult> ExecuteAsync(
        LogInput input,
        CancellationToken ct)
    {
        Console.WriteLine($"[Action] {Key} → {input.Message}");

        return Task.FromResult(ActionResults.None());
    }
}
```

***

## ⚡ Learn More

➡️ <https://www.nuget.org/packages?q=WJb>  
➡️ <https://github.com/UkrGuru/WJb.Demo>

***

## 🎁 Support WJb

If you like this project:

👉 <https://ko-fi.com/ukrguru>

Early supporters (before August 1, 2026):

👉 🎁 **FREE Solo License**

