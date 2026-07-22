# ⚡ WJb Quick Start

This is the fastest way to understand how WJb works.

---

## 🧠 What you will see

```text
send-email → log → done
```

A simple job workflow where:

- one job is enqueued
- the action performs work
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
using WJb;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== WJb Quick Start ===\n");

Console.WriteLine($"""
Workflow:
{Actions.SendEmail} → {Actions.Log} → done

""");

var store = new InMemoryStore();

// Configure actions and services
var wjb = WJbBuilder.Create(store, cfg =>
{
    cfg.AddAction<SendEmailAction>(Actions.SendEmail);
    cfg.AddAction<LogAction>(Actions.Log);

    cfg.AddService(new SmtpSettings
    {
        Host = "smtp.local"
    });
});

// Enqueue first job
Console.WriteLine($"[App] Enqueue: {Actions.SendEmail}");

await wjb.EnqueueAsync(Actions.SendEmail, new EmailInput { To = "user@test.com" });

// Execute all pending jobs
Console.WriteLine("[App] Start execution...\n");

await wjb.ExecuteLoopAsync();

Console.WriteLine("\n=== Completed ===");

public static class Actions
{
    public const string SendEmail = "send-email";
    public const string Log = "log";
}

// Action: SendEmailAction
public sealed class SendEmailAction(SmtpSettings smtp)
    : JobAction<EmailInput>
{
    private readonly SmtpSettings _smtp = smtp;

    public override Task<ActionResult> ExecuteAsync(EmailInput input, CancellationToken ct)
    {
        Console.WriteLine($"[Action] {Actions.SendEmail} → {input.To} via {_smtp.Host}");

        return Task.FromResult(ActionResults.Next(new JobCommand(
            Actions.Log, new LogInput { Message = $"Email sent to {input.To}" })));
    }
}

// Action: LogAction
public sealed class LogAction : JobAction<LogInput>
{
    public override Task<ActionResult> ExecuteAsync(LogInput input, CancellationToken ct)
    {
        Console.WriteLine($"[Action] {Actions.Log} → {input.Message}");

        return Task.FromResult(ActionResults.None());
    }
}

// Input: EmailInput
public sealed class EmailInput
{
    public string? To { get; set; }
}

// Input: LogInput
public sealed class LogInput
{
    public string? Message { get; set; }
}

// Service: SmtpSettings
public sealed class SmtpSettings
{
    public string Host { get; set; } = default!;
}
```

---

## ✅ Output

```text
=== WJb Quick Start ===

Workflow:
send-email → log → done

[App] Enqueue: send-email
[App] Start execution...

[Action] send-email → user@test.com via smtp.local
[Action] log → Email sent to user@test.com

=== Completed ===
```

---

## 💡 What this demonstrates

- Actions contain business logic
- Actions can use dependency injection
- Services are resolved automatically
- Each action explicitly defines the next step
- Workflows are deterministic and visible
- Jobs can be executed through a store-backed runtime

👉 You always know what happens and why.

---

## 🔥 Key idea

```csharp
return ActionResults.Next(new JobCommand(
    Actions.Log, new LogInput { Message = $"Email sent to {input.To}" }));
```

👉 The workflow is defined in code, not hidden in the framework.

---

## ⚡ Learn more

➡️ https://www.nuget.org/packages?q=wjb

➡️ https://github.com/UkrGuru/WJb.Demo

---

## 🎁 Support WJb

If you like this idea:

👉 https://ko-fi.com/ukrguru

Early supporters (before August 1, 2026):

👉 🎁 **FREE Solo License**