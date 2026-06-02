using WJb.Abstractions;
using WJb.Core;

namespace WJb.Demo.Full.Actions;

public class DemoPayload
{
    public int DelayMs { get; set; }

    public string Text { get; set; } = "";
}

public class DemoAction : IJobAction<DemoPayload>
{
    public async Task ExecuteAsync(JobContext ctx, DemoPayload payload)
    {
        int steps = 10;

        for (int i = 1; i <= steps; i++)
        {
            ctx.Cancellation.ThrowIfCancellationRequested();

            await Task.Delay(payload.DelayMs / steps, ctx.Cancellation);

            int progress = i * 100 / steps;

            ctx.ReportProgress(progress, $"Working... {progress}%");
        }

        ctx.Complete($"Processed: {payload.Text}");
    }
}
