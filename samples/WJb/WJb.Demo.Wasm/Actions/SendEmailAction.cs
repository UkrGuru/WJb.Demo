using WJb;
using WJb.Demo.Wasm.Actions;

public sealed class SendEmailAction : JobAction<EmailInput>
{
    public const string Key = "send-email";

    public override async Task<ActionResult> ExecuteAsync(EmailInput input, CancellationToken ct)
    {
        ReportProgress(100, "Email delivered.");

        await Task.Delay(300, ct);

        return ActionResults.Next(new JobCommand(
            LogAction.Key,
            new LogInput
            {
                Message = $"Email sent to {input.To}"
            }));
    }
}