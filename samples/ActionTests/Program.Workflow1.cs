using WJb;
using WJb.Contracts;
using WJb.Core;

var store = new InMemoryStore();

var factory = WJbConfig.Create(cfg =>
{
    cfg.AddAction<SendEmailAction>();
    cfg.AddAction<LogAction>();

    cfg.AddSetting(new SmtpSettings { Host = "smtp.local" });
});

var executor = new JobExecutor(store, factory);

await executor.EnqueueAsync<SendEmailAction>(new EmailInput { To = "test@test.com" });

await executor.ExecuteLoopAsync();


public class EmailInput
{
    public string? To { get; set; }
}

public class LogInput
{
    public string? Message { get; set; }
}

public class SendEmailAction : JobAction<EmailInput>
{
    private readonly SmtpSettings? _smtp;

    public SendEmailAction(SmtpSettings? smtp)
    {
        _smtp = smtp;
    }

    public override Task<ActionResult> ExecuteAsync(
        EmailInput input,
        CancellationToken ct)
    {
        var host = _smtp?.Host;
        var to = input.To;

        Console.WriteLine($"SMTP: {host}, To: {to}");

        return Task.FromResult(
            ActionResults.Next(
                JobCommands.Next<LogAction>(new LogInput
                {
                    Message = $"Email sent to {to}"
                })
            )
        );
    }
}


public class LogAction : JobAction<LogInput>
{
    public override Task<ActionResult> ExecuteAsync(
        LogInput input,
        CancellationToken ct = default)
    {
        Console.WriteLine(input.Message ?? "<no message>");

        return Task.FromResult(ActionResults.None());
    }
}

public class SmtpSettings
{
    public string Host { get; set; } = default!;
}
