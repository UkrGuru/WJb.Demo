using WJb;

public sealed class ProgressPayload
{
    public int DelayMs { get; set; } = 300;
}

public sealed class ProgressAction 
    : JobAction<ProgressPayload>, IProgressAction
{
    public const string Key = "progress";

    public override async Task<ActionResult> ExecuteAsync(
        ProgressPayload input, CancellationToken ct)
    {
        for (var i = 0; i <= 100; i += 25)
        {
            ct.ThrowIfCancellationRequested();

            ReportProgress(i, $"Processing {i}%");

            await Task.Delay(input.DelayMs, ct);
        }

        ReportProgress(100, "Completed ✅");

        return ActionResults.None();
    }
}