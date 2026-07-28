using WJb;
using Xunit;

namespace WJb.IntegrationTests;

/// <summary>
/// Demonstrates delayed execution.
/// Equivalent to Hangfire BackgroundJob.Schedule().
/// </summary>
public class _02_DelayedJobTests
{
    [Fact]
    public async Task Should_Run_After_Delay()
    {
        var executed = false;

        var store = new InMemoryStore();

        var runtime = WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<TestAction>("test");
        });

        TestAction.Executed = () => executed = true;

        await runtime.EnqueueAsync("test",
            new TestInput(),
            new JobOptions
            {
                Delay = TimeSpan.FromMinutes(5)
            });

        await runtime.ExecuteOnceAsync();

        Assert.False(executed);

        var pending = await store.GetJobsAsync(
            new JobQueryInfo
            {
                Status = JobStatus.Pending
            });

        Assert.Single(pending);

        var completed = await store.GetJobsAsync(
            new JobQueryInfo
            {
                Status = JobStatus.Completed
            });

        Assert.Empty(completed);
    }

    public sealed class TestInput
    {
    }

    public sealed class TestAction : JobAction<TestInput>
    {
        public static Action? Executed;

        public override Task<ActionResult> ExecuteAsync(TestInput input, CancellationToken ct = default)
        {
            Executed?.Invoke();

            return Task.FromResult(ActionResults.None());
        }
    }
}