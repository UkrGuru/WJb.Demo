using WJb.Contracts;

namespace WJb.Demo.Wasm.Actions;

public sealed class RetryEmailInput
{
    public string? To { get; set; }
}

public sealed class RetryEmailAction : JobAction<RetryEmailInput>
{
    private static int _attempts;

    public override async Task<ActionResult> ExecuteAsync(
        RetryEmailInput input,
        CancellationToken ct)
    {
        _attempts++;

        ReportProgress(
            50,
            $"Attempt {_attempts}");

        await Task.Delay(500, ct);

        if (_attempts == 1)
        {
            throw new InvalidOperationException(
                "SMTP temporarily unavailable.");
        }

        ReportProgress(
            100,
            "Email delivered.");

        _attempts = 0;

        return ActionResults.Next(
            JobCommands.Next<LogAction>(
                new LogInput
                {
                    Message =
                        $"Email sent to {input.To}"
                }));
    }
}