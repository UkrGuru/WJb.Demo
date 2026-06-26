using WJb;
using WJb.Contracts;

var factory = WJbConfig.Create(cfg =>
{
    cfg.AddAction<SendEmailAction>();
    cfg.AddSetting(new SmtpSettings { Host = "smtp.local" });
});

var action = await factory.CreateAsync<SendEmailAction>();

await action.ExecuteAsync(new EmailInput { To = "test@test.com" });

public class SendEmailAction(SmtpSettings? smtp) : JobAction<EmailInput>
{
    private readonly SmtpSettings? _smtp = smtp;

    public override Task<ActionResult> ExecuteAsync(
        EmailInput input,
        CancellationToken ct = default)
    {
        var host = _smtp?.Host ?? "<not configured>";
        var to = input.To ?? "<no recipient>";

        Console.WriteLine($"SMTP: {host}, To: {to}");

        return Task.FromResult(ActionResults.None());
    }
}

public class EmailInput
{
    public string? To { get; set; }
}

public class SmtpSettings
{
    public string Host { get; set; } = default!;
}
