using WJb;

namespace WJbPro.Demos.Actions;

public sealed class RetryEmailInput
{
    public string? To { get; set; }
}

public sealed class RetryEmailAction : JobAction<RetryEmailInput>
{
    public const string Key = "retry-email";

    private static int _attempts;

    public override async Task<IActionResult> ExecuteAsync(
        RetryEmailInput input, CancellationToken ct)
    {
        _attempts++;

        ReportProgress(50, $"Attempt {_attempts}");

        await Task.Delay(500, ct);

        if (_attempts == 1)
            throw new InvalidOperationException(
                "SMTP temporarily unavailable.");

        ReportProgress(100, "Email delivered.");

        _attempts = 0;

        return await NextAsync(new JobCommand(LogAction.Key,
            new LogInput { Message = $"Email sent to {input.To}" }));
    }
}

public sealed class LogInput
{
    public string? Message { get; set; }
}

public sealed class LogAction : JobAction<LogInput>
{
    public const string Key = "log";

    public override async Task<IActionResult> ExecuteAsync(
        LogInput input, CancellationToken ct)
    {
        var message = input.Message ?? "<empty>";

        ReportProgress(30, "Preparing log...");

        await Task.Delay(200, ct);

        ReportProgress(70, "Writing log...");

        await Task.Delay(300, ct);

        ReportProgress(100, message);

        return await CompleteAsync();
    }
}