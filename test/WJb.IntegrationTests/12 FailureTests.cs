using WJb;
using Xunit;

namespace WJb.IntegrationTests;

/// <summary>
/// Demonstrates failure handling.
/// </summary>
public class _12_FailureTests
{
    [Fact]
    public async Task Should_Mark_Job_As_Failed()
    {
        var store = new InMemoryStore();

        var runtime = WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<FailureAction>("fail");
        });

        var jobId = await runtime.EnqueueAsync(
            "fail",
            new FailureInput());

        await runtime.ExecuteOnceAsync();

        var job = await store.GetJobAsync(jobId);

        Assert.NotNull(job);
        Assert.Equal(JobStatus.Failed, job!.Status);
    }

    public sealed class FailureInput;

    public sealed class FailureAction
        : JobAction<FailureInput>
    {
        public override Task<ActionResult> ExecuteAsync(
            FailureInput input,
            CancellationToken ct = default)
        {
            throw new InvalidOperationException("Boom");
        }
    }
}