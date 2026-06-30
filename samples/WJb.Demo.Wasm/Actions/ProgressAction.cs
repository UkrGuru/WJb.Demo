using WJb.Contracts;

namespace WJb.Demo.Wasm.Actions;

public sealed class ProgressPayload
{
    public int DelayMs { get; set; } = 300;
}

public sealed class ProgressAction : JobAction<ProgressPayload>
{
    public override async Task<ActionResult> ExecuteAsync(
        ProgressPayload input, CancellationToken ct = default)
    {
        for (int i = 0; i <= 100; i += 25)
        {
            ct.ThrowIfCancellationRequested();

            ReportProgress(i, $"Processing {i}%");

            await Task.Delay(input.DelayMs, ct);
        }

        ReportProgress(100, "Completed ✅");

        return ActionResults.None();
    }
}