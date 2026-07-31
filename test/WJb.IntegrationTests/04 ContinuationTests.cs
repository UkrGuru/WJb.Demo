
namespace WJb.IntegrationTests;

/// <summary>
/// Demonstrates continuation jobs.
/// Equivalent to Hangfire ContinueWith().
/// </summary>
public class _04_ContinuationTests
{
    [Fact]
    public async Task Should_Run_Next_Action()
    {
        ActionA.Executed = false;
        ActionB.Executed = false;

        var store = new InMemoryStore();

        var runtime = WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<ActionA>("action-a");
            cfg.AddAction<ActionB>("action-b");
        });

        await runtime.EnqueueAsync(
            "action-a",
            new ActionAInput());

        await runtime.ExecuteOnceAsync();

        Assert.True(ActionA.Executed);
        Assert.False(ActionB.Executed);

        var jobs = await store.GetJobsAsync();

        var pending =
            jobs.Single(x => x.Status == JobStatus.Pending);

        Assert.Equal("action-b", pending.Action);

        await runtime.ExecuteOnceAsync();

        Assert.True(ActionB.Executed);

        jobs = await store.GetJobsAsync();

        Assert.Equal(
            2,
            jobs.Count(x => x.Status == JobStatus.Completed));
    }

    public sealed class ActionAInput
    {
    }

    public sealed class ActionA : JobAction<ActionAInput>
    {
        public static bool Executed { get; set; }

        public override Task<ActionResult> ExecuteAsync(
            ActionAInput input,
            CancellationToken ct = default)
        {
            Executed = true;

            return Task.FromResult(
                ActionResults.Next(
                    new JobCommand("action-b")));
        }
    }

    public sealed class ActionB : JobAction<object>
    {
        public static bool Executed { get; set; }

        public override Task<ActionResult> ExecuteAsync(
            object input,
            CancellationToken ct = default)
        {
            Executed = true;

            return Task.FromResult(ActionResults.None());
        }
    }
}