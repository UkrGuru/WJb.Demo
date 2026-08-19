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