namespace WJb.Demo.Wasm.Actions;

public sealed class LogInput
{
    public string Message { get; set; } = string.Empty;
}

[ActionName("log")]
public sealed class LogAction
    : JobAction<LogInput>
{
    public override async Task<IActionResult> ExecuteAsync(
        LogInput input,
        CancellationToken ct)
    {
        var message =
            string.IsNullOrWhiteSpace(input.Message)
                ? "<empty>"
                : input.Message;

        ReportProgress(
            30,
            "Preparing log...");

        await Task.Delay(
            200,
            ct);

        ReportProgress(
            70,
            "Writing log...");

        await Task.Delay(
            300,
            ct);

        ReportProgress(
            100,
            message);

        return await CompleteAsync();
    }
}

public sealed class ErrorLogInput
{
    public string Message { get; set; } = string.Empty;
}

[ActionName("error-log")]
public sealed class ErrorLogAction
    : JobAction<ErrorLogInput>
{
    public override async Task<IActionResult> ExecuteAsync(
        ErrorLogInput input,
        CancellationToken ct)
    {
        var message =
            string.IsNullOrWhiteSpace(input.Message)
                ? "<no message>"
                : input.Message;

        ReportProgress(
            30,
            "Preparing error log...");

        await Task.Delay(
            200,
            ct);

        ReportProgress(
            70,
            "Writing error log...");

        await Task.Delay(
            300,
            ct);

        ReportProgress(
            100,
            $"❌ {message}");

        return await CompleteAsync();
    }
}