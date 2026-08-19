using WJb;

namespace WJbPro.Demos.Actions;

public sealed class EmailInput
{
    public string? To { get; set; }

    public string? Subject { get; set; }
}

public sealed class SmtpSettings
{
    public const string Key = "smtp";

    public string? Host { get; set; }

    public int Port { get; set; }

    public string? From { get; set; }
}

public sealed class ConfiguredAction(SmtpSettings? smtp)
    : JobAction<EmailInput>
{
    public const string Key = "configured";

    private readonly SmtpSettings? _smtp = smtp;

    public override Task<IActionResult> ExecuteAsync(
        EmailInput input, CancellationToken ct = default)
    {
        var host = _smtp?.Host ?? "<not configured>";
        var to = input.To ?? "<no recipient>";

        var message = $"SMTP: {host}, To: {to}";

        ReportProgress(100, message);

        return CompleteAsync();
    }
}
