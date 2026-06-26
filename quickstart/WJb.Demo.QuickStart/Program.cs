using System.Text;
using WJb;
using WJb.Contracts;
using WJb.Core;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("=== WJb QuickStart ===\n");

Console.WriteLine("""
Flow:
SendEmail → Log → Done

""");

// store + factory
var store = new InMemoryStore();

var factory = WJbConfig.Create(cfg =>
{
    cfg.AddAction<SendEmailAction>();
    cfg.AddAction<LogAction>();
});

var executor = new JobExecutor(store, factory);

// enqueue first job
Console.WriteLine("[App] Enqueue: SendEmail");

await executor.EnqueueAsync<SendEmailAction>(
    new EmailInput { To = "user@test.com" }
);

// run loop (controlled)
Console.WriteLine("[App] Start execution...\n");

await executor.ExecuteLoopAsync();

Console.WriteLine("\n=== Done ===");
Console.WriteLine("All steps were explicitly defined.");

// Action: SendEmailAction
public class SendEmailAction : JobAction<EmailInput>
{
    public override Task<ActionResult> ExecuteAsync(
        EmailInput input,
        CancellationToken ct)
    {
        Console.WriteLine($"[Action] SendEmail → {input.To}");

        return Task.FromResult(
            ActionResults.Next(
                JobCommands.Next<LogAction>(
                    new LogInput
                    {
                        Message = $"Email sent to {input.To}"
                    })
            )
        );
    }
}

// Action: LogAction
public class LogAction : JobAction<LogInput>
{
    public override Task<ActionResult> ExecuteAsync(
        LogInput input,
        CancellationToken ct)
    {
        Console.WriteLine($"[Action] Log → {input.Message}");

        return Task.FromResult(ActionResults.None());
    }
}

// Input: EmailInput
public class EmailInput
{
    public string? To { get; set; }
}

// Input: LogInput
public class LogInput
{
    public string? Message { get; set; }
}
