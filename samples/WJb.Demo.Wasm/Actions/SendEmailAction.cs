using WJb;
using WJb.Demo.Wasm.Actions;

public sealed class SendEmailAction : JobAction<EmailInput>
{
    public const string Key = "send-email";

    public override Task<ActionResult> ExecuteAsync(
        EmailInput input,
        CancellationToken ct)
    {
        ReportProgress(
            100,
            "Email delivered.");

        return Task.FromResult(
            ActionResults.Next(
                new JobCommand(
                    LogAction.Key,
                    new LogInput
                    {
                        Message = $"Email sent to {input.To}"
                    })));
    }
}