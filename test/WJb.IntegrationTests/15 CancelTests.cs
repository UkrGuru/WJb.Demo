using WJb;
using Xunit;

namespace WJb.IntegrationTests;

/// <summary>
/// Demonstrates job cancellation.
/// </summary>
public class _15_CancelTests
{
    [Fact]
    public async Task Should_Cancel_Running_Job()
    {
        var store = new InMemoryStore();

        var runtime = WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<LongRunningAction>("long-running");
        });

        var jobId = await runtime.EnqueueAsync(
            "long-running",
            new LongRunningInput());

        var execution = Task.Run(
            () => runtime.ExecuteLoopAsync());

        await Task.Delay(50);

        var cancelled =
            runtime.TryCancel(jobId);

        await execution;

        Assert.True(cancelled);

        var job = await store.GetJobAsync(jobId);

        Assert.NotNull(job);
        Assert.Equal(JobStatus.Failed, job!.Status);
    }

    public sealed class LongRunningInput;

    public sealed class LongRunningAction
        : JobAction<LongRunningInput>
    {
        public override async Task<ActionResult> ExecuteAsync(
            LongRunningInput input,
            CancellationToken ct = default)
        {
            await Task.Delay(
                TimeSpan.FromSeconds(5),
                ct);

            return ActionResults.None();
        }
    }
}