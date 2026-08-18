namespace WJb.Demo.Wasm.Actions;

[ActionName("send-email")]
public sealed class SendEmailAction
    : JobAction<EmailInput>
{
    public override async Task<IActionResult> ExecuteAsync(
        EmailInput input,
        CancellationToken ct)
    {
        ReportProgress(100, "Email delivered.");

        await Task.Delay(300, ct);

        return await NextAsync<LogAction>(
            new LogInput
            {
                Message = $"Email sent to {input.To}"
            });
    }
}