# ⚡ WJb Quick Start

This is the fastest way to understand how WJb works.

---

## 🧠 What you will see

```text
send-email → log → done
```

A simple workflow where:

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

var store = new InMemoryStore();

var wjb = WJbBuilder.Create(store, cfg =>
{
    cfg.AddAction<SendEmailAction>(Actions.SendEmail);
    cfg.AddAction<LogAction>(Actions.Log);

    cfg.AddService(new SmtpSettings
    {
        Host = "smtp.local"
    });
});

Console.WriteLine("=== WJb Quick Start ===\n");

Console.WriteLine($"""
Workflow: {Actions.SendEmail} → {Actions.Log} → done
""");

Console.WriteLine($"[App] Enqueue: {Actions.SendEmail}");

await wjb.EnqueueAsync(Actions.SendEmail,
    new EmailInput { To = "user@test.com" });

Console.WriteLine("[App] Start execution...\n");

await wjb.ExecuteLoopAsync();

Console.WriteLine("\n=== Completed ===");

public static class Actions
{
    public const string SendEmail = "send-email";
    public const string Log = "log";
}

[ActionName(Actions.SendEmail)]
public sealed class SendEmailAction(SmtpSettings smtp)
    : JobAction<EmailInput>
{
    public override Task<IActionResult> ExecuteAsync(
        EmailInput input, CancellationToken ct)
    {
        Console.WriteLine( $"[Action] {Actions.SendEmail} -> {input.To} via {smtp.Host}");

        return NextAsync<LogAction>(
            new LogInput { Message = $"Email sent to {input.To}" });
    }
}

[ActionName(Actions.Log)]
public sealed class LogAction : JobAction<LogInput>
{
    public override Task<IActionResult> ExecuteAsync(
        LogInput input, CancellationToken ct)
    {
        Console.WriteLine($"[Action] {Actions.Log} -> {input.Message}");

        return CompleteAsync();
    }
}

public sealed class EmailInput
{
    public string To { get; set; } = string.Empty;
}

public sealed class LogInput
{
    public string Message { get; set; } = string.Empty;
}

public sealed class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
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

[Action] send-email -> user@test.com via smtp.local
[Action] log -> Email sent to user@test.com

=== Completed ===
```

---

## 💡 What this demonstrates

- Actions contain business logic
- Actions can use dependency injection
- Services are resolved automatically
- Actions explicitly define what runs next
- Workflows are deterministic and visible
- Results describe workflow outcomes
- Jobs execute through a store-backed runtime

👉 You always know what happens and why.

---

## 🔥 Key Idea

```csharp
return NextAsync<LogAction>(
    new LogInput { Message = $"Email sent to {input.To}" });
```

👉 The workflow is defined in code.

---

## ⚡ Learn More

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
