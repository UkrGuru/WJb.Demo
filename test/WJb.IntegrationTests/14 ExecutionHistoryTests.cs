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

        var jobs = await store.GetJobsAsync();

        var completed =
            jobs.Count(x => x.Status == JobStatus.Completed);

        var failed =
            jobs.Count(x => x.Status == JobStatus.Failed);

        Assert.Equal(1, completed);

        Assert.Equal(2, failed);

        Assert.Equal(
            3,
            completed + failed);
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