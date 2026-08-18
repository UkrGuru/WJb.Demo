namespace WJb.Demo.Wasm.Actions;

public sealed class ProgressPayload
{
    public int DelayMs { get; set; } = 300;
}

[ActionName("progress")]
public sealed class ProgressAction
    : JobAction<ProgressPayload>, IProgressAction
{
    public override async Task<IActionResult> ExecuteAsync(
        ProgressPayload input,
        CancellationToken ct)
    {
        for (var i = 0; i <= 100; i += 25)
        {
            ct.ThrowIfCancellationRequested();

            ReportProgress(
                i,
                $"Processing {i}%");

            await Task.Delay(
                input.DelayMs,
                ct);
        }

        ReportProgress(
            100,
            "Completed ✅");

        return await CompleteAsync();
    }
}