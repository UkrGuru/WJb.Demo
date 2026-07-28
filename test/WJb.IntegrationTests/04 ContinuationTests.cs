using WJb;
using Xunit;

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

        await runtime.EnqueueAsync("action-a", new ActionAInput());

        await runtime.ExecuteOnceAsync();

        Assert.True(ActionA.Executed);
        Assert.False(ActionB.Executed);

        var pending = await store.GetJobsAsync(
            new JobQueryInfo
            {
                Status = JobStatus.Pending
            });

        Assert.Single(pending);
        Assert.Equal("action-b", pending[0].Action);

        await runtime.ExecuteOnceAsync();

        Assert.True(ActionB.Executed);

        var completed = await store.GetJobsAsync(
            new JobQueryInfo
            {
                Status = JobStatus.Completed
            });

        Assert.Equal(2, completed.Count);
    }

    public sealed class ActionAInput
    {
    }

    public sealed class ActionA : JobAction<ActionAInput>
    {
        public static bool Executed { get; set; }

        public override Task<ActionResult> ExecuteAsync(ActionAInput input, CancellationToken ct = default)
        {
            Executed = true;

            return Task.FromResult(ActionResults.Next(new JobCommand("action-b")));
        }
    }

    public sealed class ActionB : JobAction<object>
    {
        public static bool Executed { get; set; }

        public override Task<ActionResult> ExecuteAsync(object input, CancellationToken ct = default)
        {
            Executed = true;

            return Task.FromResult(ActionResults.None());
        }
    }
}