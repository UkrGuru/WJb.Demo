using WJb;

public sealed class LogInput
{
    public string? Message { get; set; }
}

public sealed class LogAction :
    JobAction<LogInput>,
    IProgressAction
{
    public const string Key = "log";

    public event Action<JobProgress>? OnProgress;

    public override async Task<ActionResult> ExecuteAsync(
        LogInput input,
        CancellationToken ct)
    {
        var message = input.Message ?? "<empty>";

        OnProgress?.Invoke(new JobProgress
        {
            Progress = 30,
            Message = "Preparing log..."
        });

        await Task.Delay(200, ct);

        OnProgress?.Invoke(new JobProgress
        {
            Progress = 70,
            Message = "Writing log..."
        });

        await Task.Delay(300, ct);

        OnProgress?.Invoke(new JobProgress
        {
            Progress = 100,
            Message = message
        });

        return ActionResults.None();
    }
}

public sealed class ErrorLogInput
{
    public string? Message { get; set; }
}

public sealed class ErrorLogAction :
    JobAction<ErrorLogInput>,
    IProgressAction
{
    public const string Key = "error-log";

    public event Action<JobProgress>? OnProgress;

    public override async Task<ActionResult> ExecuteAsync(
        ErrorLogInput input,
        CancellationToken ct)
    {
        var message = input.Message ?? "<no message>";

        OnProgress?.Invoke(new JobProgress
        {
            Progress = 30,
            Message = "Preparing error log..."
        });

        await Task.Delay(200, ct);

        OnProgress?.Invoke(new JobProgress
        {
            Progress = 70,
            Message = "Writing error log..."
        });

        await Task.Delay(300, ct);

        OnProgress?.Invoke(new JobProgress
        {
            Progress = 100,
            Message = $"❌ {message}"
        });

        return ActionResults.None();
    }
}