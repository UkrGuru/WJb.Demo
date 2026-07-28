namespace WJb.IntegrationTests;

/// <summary>
/// Demonstrates fan-out workflow execution.
/// </summary>
public class _06_FanOutTests
{
    [Fact]
    public async Task Should_Run_Several_Next_Actions()
    {
        FanOutState.Executed.Clear();

        var store = new InMemoryStore();

        var runtime = WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<ImportAction>("import");
            cfg.AddAction<ValidateAction>("validate");
            cfg.AddAction<NotifyAction>("notify");
            cfg.AddAction<AuditAction>("audit");
        });

        await runtime.EnqueueAsync("import", new ImportInput());

        await runtime.ExecuteLoopAsync();

        Assert.Contains("Import", FanOutState.Executed);
        Assert.Contains("Validate", FanOutState.Executed);
        Assert.Contains("Notify", FanOutState.Executed);
        Assert.Contains("Audit", FanOutState.Executed);

        var completed = await store.GetJobsAsync(
            new JobQueryInfo
            {
                Status = JobStatus.Completed
            });

        Assert.Equal(4, completed.Count);
    }

    public sealed class ImportInput;

    public sealed class ImportAction
        : JobAction<ImportInput>
    {
        public override Task<ActionResult> ExecuteAsync(ImportInput input, CancellationToken ct = default)
        {
            FanOutState.Executed.Add("Import");

            return Task.FromResult(
                ActionResults.Next(
                    new JobCommand("validate"),
                    new JobCommand("notify"),
                    new JobCommand("audit")));
        }
    }

    public sealed class ValidateAction
        : JobAction<object>
    {
        public override Task<ActionResult> ExecuteAsync(object input, CancellationToken ct = default)
        {
            FanOutState.Executed.Add("Validate");

            return Task.FromResult(ActionResults.None());
        }
    }

    public sealed class NotifyAction : JobAction<object>
    {
        public override Task<ActionResult> ExecuteAsync(object input, CancellationToken ct = default)
        {
            FanOutState.Executed.Add("Notify");

            return Task.FromResult(ActionResults.None());
        }
    }

    public sealed class AuditAction : JobAction<object>
    {
        public override Task<ActionResult> ExecuteAsync(object input, CancellationToken ct = default)
        {
            FanOutState.Executed.Add("Audit");

            return Task.FromResult(ActionResults.None());
        }
    }

    private static class FanOutState
    {
        public static List<string> Executed { get; } = [];
    }
}