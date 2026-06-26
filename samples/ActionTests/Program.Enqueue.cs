using WJb;
using WJb.Contracts;
using WJb.Core;

var store = new InMemoryStore();

var factory = new WJbConfig()
    .AddAction<SendEmailAction>()
    .AddSetting(new SmtpSettings { Host = "smtp.local" })
    .Build();

var executor = new JobExecutor(store, factory);

await executor.EnqueueAsync<SendEmailAction>( new { to = "test@test.com" });

await executor.ExecuteOnceAsync();

public class SendEmailAction(SmtpSettings smtp) : JobAction<EmailInput>
{
    private readonly SmtpSettings _smtp = smtp;

    public override Task<ActionResult> ExecuteAsync(EmailInput input, CancellationToken ct = default)
    {
        var host = _smtp.Host;
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
