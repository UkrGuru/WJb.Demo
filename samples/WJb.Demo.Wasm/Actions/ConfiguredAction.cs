using WJb.Contracts;

namespace WJb.Demo.Wasm.Actions;

public sealed class EmailInput
{
    public string? To { get; set; }
}

public sealed class SmtpSettings
{
    public string Host { get; set; } = default!;
}

public sealed class ConfiguredAction(SmtpSettings? smtp) : JobAction<EmailInput>
{
    private readonly SmtpSettings? _smtp = smtp;

    public override Task<ActionResult> ExecuteAsync(EmailInput input, CancellationToken ct = default)
    {
        var host = _smtp?.Host ?? "<not configured>";
        var to = input.To ?? "<no recipient>";

        var message = $"SMTP: {host}, To: {to}";

        ReportProgress(100, message);

        return Task.FromResult(ActionResults.None());
    }
}