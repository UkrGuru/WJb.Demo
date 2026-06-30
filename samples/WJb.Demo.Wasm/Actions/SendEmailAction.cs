using WJb;
using WJb.Contracts;
using WJb.Demo.Wasm.Actions;

public sealed class SendEmailAction(SmtpSettings? smtp) : JobAction<EmailInput>
{
    private readonly SmtpSettings? _smtp = smtp;

    public override async Task<ActionResult> ExecuteAsync(
        EmailInput input, CancellationToken ct)
    {
        var host = _smtp?.Host ?? "<not configured>";
        var to = input.To ?? "<no recipient>";

        try
        {
            ReportProgress(10, "Resolving SMTP...");
            await Task.Delay(200, ct);

            ReportProgress(40, "Connecting...");
            await Task.Delay(300, ct);

            ReportProgress(70, "Sending email...");
            await Task.Delay(400, ct);

            ct.ThrowIfCancellationRequested();

            ReportProgress(100, $"SMTP: {host}, To: {to}");

            return ActionResults.Next(
                JobCommands.Next<LogAction>(new LogInput
                {
                    Message = $"Email sent to {to}"
                })
            );
        }
        catch (OperationCanceledException)
        {
            return ActionResults.Next(
                JobCommands.Next<ErrorLogAction>(new ErrorLogInput
                {
                    Message = $"Email cancelled for {to}"
                })
            );
        }
        catch (Exception ex)
        {
            return ActionResults.Next(
                JobCommands.Next<ErrorLogAction>(new ErrorLogInput
                {
                    Message = $"Email failed: {ex.Message}"
                })
            );
        }
    }
}