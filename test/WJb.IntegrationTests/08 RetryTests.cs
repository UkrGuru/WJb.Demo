using WJb;
using Xunit;

namespace WJb.IntegrationTests;

/// <summary>
/// Demonstrates retry handling.
///
/// Attempt 1 => Failed
/// Attempt 2 => Failed
/// Attempt 3 => Completed
/// </summary>
public class _08_RetryTests
{
    [Fact]
    public async Task Should_Retry_Failed_Action()
    {
        RetryAction.Attempts = 0;

        var store = new InMemoryStore();

        var runtime = WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<RetryAction>("retry");
        });

        await runtime.EnqueueAsync(
            "retry",
            new RetryInput(),
            new JobOptions
            {
                MaxRetries = 2,
                RetryDelay = TimeSpan.Zero
            });

        await runtime.ExecuteLoopAsync();

        Assert.Equal(3, RetryAction.Attempts);

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

    public sealed class RetryInput
    {
    }

    public sealed class RetryAction
        : JobAction<RetryInput>
    {
        public static int Attempts { get; set; }

        public override Task<ActionResult> ExecuteAsync(
            RetryInput input,
            CancellationToken ct = default)
        {
            Attempts++;

            if (Attempts < 3)
            {
                throw new InvalidOperationException(
                    $"Attempt {Attempts} failed.");
            }

            return Task.FromResult(ActionResults.None());
        }
    }
}