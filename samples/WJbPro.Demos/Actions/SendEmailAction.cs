using WJb;

namespace WJbPro.Demos.Actions;

public sealed class SendEmailAction(SmtpSettings smtp)
    : JobAction<EmailInput>
{
    public const string Key = "send-email";

    public override Task<IActionResult> ExecuteAsync(EmailInput input, CancellationToken ct)
    {
        ReportProgress(100, "Email sent");

        return CompleteAsync();
    }
}