using System.Text;
using WJb;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("=== WJb Quick Start ===\n");

Console.WriteLine($"""
Workflow:
{SendEmailAction.Key} → {LogAction.Key} → done

""");

var store = new InMemoryStore();

var wjb = WJbBuilder.Create(cfg =>
{
    cfg.AddAction<SendEmailAction>(SendEmailAction.Key);
    cfg.AddAction<LogAction>(LogAction.Key);

    cfg.UseStore(store);
});

// enqueue first job
Console.WriteLine($"[App] Enqueue: {SendEmailAction.Key}");

await wjb.Executor.EnqueueAsync(SendEmailAction.Key, new EmailInput { To = "user@test.com" });

// run loop (controlled)
Console.WriteLine("[App] Start execution...\n");

await wjb.Executor.ExecuteLoopAsync();

Console.WriteLine("\n=== Completed ===");

// Action: SendEmailAction
public sealed class SendEmailAction : JobAction<EmailInput>
{
    public const string Key = "send-email";

    public override Task<ActionResult> ExecuteAsync(EmailInput input, CancellationToken ct)
    {
        Console.WriteLine($"[Action] {Key} → {input.To}");

        return Task.FromResult(
            ActionResults.Next(new JobCommand(LogAction.Key,
                new LogInput
                {
                    Message = $"Email sent to {input.To}"
                })
            )
        );

    }
}

// Action: LogAction
public sealed class LogAction : JobAction<LogInput>
{
    public const string Key = "log";

    public override Task<ActionResult> ExecuteAsync(LogInput input, CancellationToken ct)
    {
        Console.WriteLine($"[Action] {Key} → {input.Message}");

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