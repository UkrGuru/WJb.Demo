using WJb;
using WJb.Contracts;
using WJb.Core;

var store = new InMemoryStore();

var factory = WJbConfig.Create(cfg =>
{
    cfg.AddAction<SendEmailAction>();
    cfg.AddAction<LogAction>();
    cfg.AddAction<AuditAction>();

    cfg.AddSetting(new SmtpSettings { Host = "smtp.local" });
});

var executor = new JobExecutor(store, factory);

await executor.EnqueueAsync<SendEmailAction>(new EmailInput { To = null });

await executor.ExecuteLoopAsync();


public class EmailInput
{
    public string? To { get; set; }
}

public class LogInput
{
    public string? Message { get; set; }
}

public class AuditInput
{
    public string? Event { get; set; }
}

public class SendEmailAction(SmtpSettings? smtp) : JobAction<EmailInput>
{
    private readonly SmtpSettings? _smtp = smtp;

    public override Task<ActionResult> ExecuteAsync(EmailInput input, CancellationToken ct)
    {
        var to = input.To;

        if (to == null)
        {
            return Task.FromResult(
                ActionResults.Next(
                    JobCommands.Next<LogAction>(new LogInput
                    {
                        Message = "Missing recipient"
                    })
                )
            );
        }

        Console.WriteLine($"SMTP: {_smtp?.Host}, To: {to}");

        return Task.FromResult(
            ActionResults.Next(
                JobCommands.Next<AuditAction>(new AuditInput
                {
                    Event = $"Email sent to {to}"
                })
            )
        );
    }
}

public class LogAction : JobAction<LogInput>
{
    public override Task<ActionResult> ExecuteAsync(LogInput input, CancellationToken ct = default)
    {
        Console.WriteLine($"LOG: {input.Message}");

        return Task.FromResult(ActionResults.None());
    }
}

public class AuditAction : JobAction<AuditInput>
{
    public override Task<ActionResult> ExecuteAsync(AuditInput input, CancellationToken ct = default)
    {
        Console.WriteLine($"AUDIT: {input.Event}");

        return Task.FromResult(ActionResults.None());
    }
}

public class SmtpSettings
{
    public string Host { get; set; } = default!;
}
