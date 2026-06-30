using WJb.Contracts;

public sealed class LogInput
{
    public string? Message { get; set; }
}

public sealed class LogAction : JobAction<LogInput>
{
    public override async Task<ActionResult> ExecuteAsync(
        LogInput input, CancellationToken ct)
    {
        var message = input.Message ?? "<empty>";

        ReportProgress(30, "Preparing log...");
        await Task.Delay(200, ct);

        ReportProgress(70, "Writing log...");
        await Task.Delay(300, ct);

        ReportProgress(100, message);

        return ActionResults.None();
    }
}

public sealed class ErrorLogInput
{
    public string? Message { get; set; }
}

public sealed class ErrorLogAction : JobAction<ErrorLogInput>
{
    public override async Task<ActionResult> ExecuteAsync(
        ErrorLogInput input, CancellationToken ct)
    {
        var message = input.Message ?? "<no message>";

        ReportProgress(30, "Preparing error log...");
        await Task.Delay(200, ct);

        ReportProgress(70, "Writing error log...");
        await Task.Delay(300, ct);

        ReportProgress(100, $"❌ {message}");

        return ActionResults.None();
    }
}
