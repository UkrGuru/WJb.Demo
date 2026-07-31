namespace WJb.IntegrationTests;

/// <summary>
/// Demonstrates a multi-step workflow.
/// </summary>
public class _05_WorkflowTests
{
    [Fact]
    public async Task Should_Run_Multi_Step_Workflow()
    {
        WorkflowState.Executed.Clear();

        var store = new InMemoryStore();

        var runtime = WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<CreateReportAction>("create-report");
            cfg.AddAction<SendReportAction>("send-report");
            cfg.AddAction<ArchiveReportAction>("archive-report");
        });

        await runtime.EnqueueAsync(
            "create-report",
            new CreateReportInput());

        await runtime.ExecuteLoopAsync();

        Assert.Equal(
            [
                "CreateReport",
            "SendReport",
            "ArchiveReport"
            ],
            WorkflowState.Executed);

        var jobs = await store.GetJobsAsync();

        Assert.Equal(
            3,
            jobs.Count(x => x.Status == JobStatus.Completed));
    }

    public sealed class CreateReportInput;

    public sealed class CreateReportAction
        : JobAction<CreateReportInput>
    {
        public override Task<ActionResult> ExecuteAsync(
            CreateReportInput input,
            CancellationToken ct = default)
        {
            WorkflowState.Executed.Add("CreateReport");

            return Task.FromResult(
                ActionResults.Next(
                    new JobCommand("send-report")));
        }
    }

    public sealed class SendReportAction
        : JobAction<object>
    {
        public override Task<ActionResult> ExecuteAsync(
            object input,
            CancellationToken ct = default)
        {
            WorkflowState.Executed.Add("SendReport");

            return Task.FromResult(
                ActionResults.Next(
                    new JobCommand("archive-report")));
        }
    }

    public sealed class ArchiveReportAction
        : JobAction<object>
    {
        public override Task<ActionResult> ExecuteAsync(
            object input,
            CancellationToken ct = default)
        {
            WorkflowState.Executed.Add("ArchiveReport");

            return Task.FromResult(ActionResults.None());
        }
    }

    private static class WorkflowState
    {
        public static List<string> Executed { get; } = [];
    }
}