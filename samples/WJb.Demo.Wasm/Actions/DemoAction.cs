using WJb.Core;

public class DemoPayload
{
    public int DelayMs { get; set; }
    public string Text { get; set; } = "";
}

public class DemoAction : IJobAction<DemoPayload>
{
    public async Task ExecuteAsync(JobContext context, DemoPayload payload)
    {
        for (int i = 0; i <= 100; i += 10)
        {
            // ✅ FIX: вместо Cancellation
            context.ThrowIfCancellationRequested();

            // ✅ FIX: через Reporter
            context.Reporter.ReportProgress(i, $"Progress {i}%");

            await Task.Delay(payload.DelayMs / 10);
        }

        // ✅ FIX: через Reporter
        context.Reporter.Complete(payload.Text);
    }
}
