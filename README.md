# ⚡ WJb.Demo: Ultra-lightweight Workflow & Background Processing for .NET

Welcome! If you found this repository after seeing our benchmarks — yes, **WJb is real, and it actually delivers that performance.** 

WJb was built as a zero-dependency, low-allocation alternative to Hangfire and Quartz. It handles job lifecycles, queues, retries, and step-by-step workflow orchestration seamlessly, even inside Blazor WebAssembly.

---

## 🔥 Why WJb instead of Hangfire/Quartz?

| Feature | Hangfire / Quartz | ⚡ WJb |
| :--- | :--- | :--- |
| **Footprint & Memory** | High allocation, heavy infrastructure | **Ultra-low allocations, lightweight** |
| **Dependencies** | Requires SQL Server / Redis / Postgres | **Zero-dependency core (runs in-memory out of the box)** |
| **Workflow Support** | Hard to chain jobs without extra libraries | **✅ Native Workflow Chaining (`ActionResult.Next`)** |
| **Blazor WASM Support**| ❌ No | **✅ Yes (Full client-side support)** |
| **Configuration** | Complex DSL and Middlewares | **Native C# code without "magic"** |

---

## 🚀 Show me the code (Run in 30 seconds)

You don't need a database to test this. Just clone the repo, open the solution, and hit `F5` in the QuickStart project.

Here is how elegant and modern your workflow orchestration looks with **WJb** (Top-Level Statements example):

```csharp
using WJb;

Console.OutputEncoding = System.Text.Encoding.UTF8;

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
public sealed class SendEmailAction(SmtpSettings smtp) : JobAction<EmailInput>
{
    public override ValueTask<ActionResult> ExecuteAsync(
        EmailInput? input, CancellationToken ct = default)
    {
        Console.WriteLine(
            $"[Action] {Actions.SendEmail} -> {input?.To} via {smtp.Host}");

        return new ActionResult()
            .Next(Actions.Log, $"Email sent to {input?.To}");
    }
}

[ActionName(Actions.Log)]
public sealed class LogAction : JobAction<string?>
{
    public override ValueTask<ActionResult> ExecuteAsync(
        string? message, CancellationToken ct = default)
    {
        Console.WriteLine(
            $"[Action] {Actions.Log} -> {message}");

        return ActionResult.Empty;
    }
}

public sealed class EmailInput
{
    public string? To { get; set; }
}

public sealed class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
}
```

---

## 📁 What's Inside This Demo Repository?

Don't guess where to look. Here is your roadmap depending on your goals:

*   **⚡ [quickstart](./quickstart/WJb.Demo.QuickStart)** — The absolute bare minimum to see WJb alive. Run it to see console logging of job lifecycles.
*   **🧩 [samples](./samples)** — Real-world integration scenarios. Check this if you want to see how the **Job Monitoring UI** integrates into web apps.
*   **🏁 [benchmark](./benchmark)** — The exact source code of the benchmarks. Run them locally on your machine to verify WJb speed vs Hangfire/Quartz.
*   **🔥 [stress](./stress)** — High-load simulation. Watch how WJb handles heavy concurrent traffic without throwing OutOfMemory exceptions.

---

## 💬 Let's break the vacuum: Talk to the Author!

Hi, I am **Oleksandr Viktor**, the independent creator of WJb. 

I see many developers cloning this demo repo every day, but staying silent. Let's change that! If you are exploring WJb for your project, commercial product, or enterprise architecture, **I want to help you personally.**

*   💬 **Got a specific use case?** Open a [GitHub Discussion](https://github.com) or an Issue right now.
*   🏗️ **Need help with migration?** If you are moving away from heavy Hangfire/Quartz setups, tell me about your architecture. I can review your tech stack and give free integration advice.
*   📧 **Direct Contact:** Reach out via `ukrguru@gmail.com` or find me on LinkedIn.

**If you like the performance numbers and this clean API, please drop a ⭐ Star to this repository!** It helps other .NET developers discover that workflows don't have to be heavy.
