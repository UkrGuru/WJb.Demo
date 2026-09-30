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
public sealed class SendEmailAction(SmtpSettings smtp)
    : JobAction<EmailInput>
{
    public override ValueTask<ActionResult> ExecuteAsync(
        EmailInput? input, CancellationToken ct = default)
    {
        Console.WriteLine(
            $"[Action] {Actions.SendEmail} -> {input?.To} via {smtp.Host}");

        return ValueTask.FromResult(Results.Done()
                .Next(Actions.Log, $"Email sent to {input?.To}"));
    }
}

[ActionName(Actions.Log)]
public sealed class LogAction
    : JobAction<string?>
{
    public override ValueTask<ActionResult> ExecuteAsync(
        string? message, CancellationToken ct = default)
    {
        Console.WriteLine(
            $"[Action] {Actions.Log} -> {message}");

        return ValueTask.FromResult(Results.Done());
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
