
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace WJb.IntegrationTests;

/// <summary>
/// Demonstrates a fire-and-forget job.
/// Equivalent to Hangfire BackgroundJob.Enqueue().
/// </summary>
public class _01_FireAndForgetTests
{
    [Fact]
    public async Task Should_Run_Once()
    {
        var executed = 0;

        var store = new InMemoryStore();

        var runtime = WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<TestAction>("test");
        });

        TestAction.Executed = () => executed++;

        var jobId = await runtime.EnqueueAsync(
            "test",
            new TestInput());

        await runtime.ExecuteOnceAsync();

        Assert.Equal(1, executed);

        var executedAgain =
            await runtime.ExecuteOnceAsync();

        Assert.False(executedAgain);

        var job = await store.GetJobAsync(jobId);

        Assert.NotNull(job);
        Assert.Equal(JobStatus.Completed, job!.Status);
    }

    public sealed class TestInput
    {
    }

    public sealed class TestAction
        : JobAction<TestInput>
    {
        public static Action? Executed;

        public override Task<ActionResult> ExecuteAsync(
            TestInput input,
            CancellationToken ct = default)
        {
            Executed?.Invoke();

            return Task.FromResult(ActionResults.None());
        }
    }
}