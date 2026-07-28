using WJb;
using Xunit;

namespace WJb.IntegrationTests;

/// <summary>
/// Demonstrates progress reporting.
/// </summary>
public class _10_ProgressTests
{
    [Fact]
    public async Task Should_Save_Progress()
    {
        var store = new InMemoryStore();

        var runtime = WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<ProgressAction>("progress");
        });

        var jobId = await runtime.EnqueueAsync(
            "progress",
            new ProgressInput());

        await runtime.ExecuteOnceAsync();

        var job = await store.GetJobAsync(jobId);

        Assert.NotNull(job);
        Assert.Equal(JobStatus.Completed, job!.Status);
        Assert.Equal(100, job.Progress);
        Assert.Equal("Done", job.Message);
    }

    public sealed class ProgressInput;

    public sealed class ProgressAction
        : JobAction<ProgressInput>
    {
        public override async Task<ActionResult> ExecuteAsync(
            ProgressInput input,
            CancellationToken ct = default)
        {
            ReportProgress(25, "Starting");

            await Task.Delay(1, ct);

            ReportProgress(50, "Working");

            await Task.Delay(1, ct);

            ReportProgress(100, "Done");

            return ActionResults.None();
        }
    }
}