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

    cfg.AddService(new SmtpSettings { Host = "smtp.local" });
});

// Enqueue first job
Console.WriteLine($"[App] Enqueue: {Actions.SendEmail}");

await wjb.EnqueueAsync(
    Actions.SendEmail, 
    new EmailInput { To = "user@test.com" });

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
public sealed class SendEmailAction(SmtpSettings smtp) : JobAction<EmailInput>
{
    private readonly SmtpSettings _smtp = smtp;

    public override Task<ActionResult> ExecuteAsync(EmailInput input, CancellationToken ct)
    {
        Console.WriteLine($"[Action] {Actions.SendEmail} → {input.To} via {_smtp.Host}");

        return Task.FromResult(
            ActionResults.Next(new JobCommand(
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
public class SmtpSettings
{
    public string Host { get; set; } = default!;
}
