using WJb;
using Xunit;

namespace WJb.IntegrationTests;

/// <summary>
/// Demonstrates continuation after failure.
/// </summary>
public class _13_FailureContinuationTests
{
    [Fact]
    public async Task Should_Run_Compensation_After_Failure()
    {
        CompensationAction.Executed = false;

        var store = new InMemoryStore();

        var runtime = WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<FailingAction>("fail");
            cfg.AddAction<CompensationAction>("compensate");
        });

        await runtime.EnqueueAsync(
            "fail",
            new FailureInput(),
            new JobOptions
            {
                OnFailure = "compensate"
            });

        await runtime.ExecuteLoopAsync();

        Assert.True(CompensationAction.Executed);
    }

    public sealed class FailureInput;

    public sealed class FailingAction : JobAction<FailureInput>
    {
        public override Task<ActionResult> ExecuteAsync(
            FailureInput input,
            CancellationToken ct = default)
        {
            throw new InvalidOperationException(
                "Something went wrong.");
        }
    }

    public sealed class CompensationAction : JobAction<object>
    {
        public static bool Executed { get; set; }

        public override Task<ActionResult> ExecuteAsync(
            object input,
            CancellationToken ct = default)
        {
            Executed = true;

            return Task.FromResult(
                ActionResults.None());
        }
    }
}