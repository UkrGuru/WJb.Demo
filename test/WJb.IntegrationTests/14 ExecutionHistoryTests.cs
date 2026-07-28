using WJb;
using Xunit;

namespace WJb.IntegrationTests;

/// <summary>
/// Demonstrates execution history preservation.
/// </summary>
public class _14_ExecutionHistoryTests
{
    [Fact]
    public async Task Should_Preserve_Execution_History()
    {
        HistoryAction.Attempts = 0;

        var store = new InMemoryStore();

        var runtime = WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<HistoryAction>("history");
        });

        await runtime.EnqueueAsync(
            "history",
            new HistoryInput(),
            new JobOptions
            {
                MaxRetries = 2,
                RetryDelay = TimeSpan.Zero
            });

        await runtime.ExecuteLoopAsync();

        var completed = await store.GetJobsAsync(
            new JobQueryInfo
            {
                Status = JobStatus.Completed
            });

        var failed = await store.GetJobsAsync(
            new JobQueryInfo
            {
                Status = JobStatus.Failed
            });

        Assert.Single(completed);
        Assert.Equal(2, failed.Count);

        Assert.Equal(
            3,
            completed.Count + failed.Count);
    }

    public sealed class HistoryInput;

    public sealed class HistoryAction
        : JobAction<HistoryInput>
    {
        public static int Attempts { get; set; }

        public override Task<ActionResult> ExecuteAsync(
            HistoryInput input,
            CancellationToken ct = default)
        {
            Attempts++;

            if (Attempts < 3)
                throw new InvalidOperationException();

            return Task.FromResult(
                ActionResults.None());
        }
    }
}