namespace WJb.IntegrationTests;

/// <summary>
/// Available only in the commercial edition.
/// Demonstrates queue-based execution.
/// Equivalent to dedicated workers in Hangfire / MassTransit.
/// </summary>
public class _09_QueueTests
{
    [Fact]
    public async Task Should_Execute_Only_Matching_Queue()
    {
        //EmailAction.Executed = false;
        //SmsAction.Executed = false;

        //var store = new InMemoryStore();

        //var runtime = WJbBuilder.Create(store, cfg =>
        //{
        //    cfg.AddAction<EmailAction>("email");
        //    cfg.AddAction<SmsAction>("sms");
        //});


        //await runtime.EnqueueAsync(
        //    "email", new QueueInput(), new JobOptions { Queue = "emails" });

        //await runtime.EnqueueAsync(
        //    "sms", new QueueInput(), new JobOptions { Queue = "sms" });

        //await runtime.ExecuteOnceAsync("emails");

        //Assert.True(EmailAction.Executed);
        //Assert.False(SmsAction.Executed);

        //var pending = await store.GetJobsAsync(new JobQueryInfo { Status = JobStatus.Pending });

        //Assert.Single(pending);
        //Assert.Equal("sms", pending[0].Action);
    }

    public sealed class QueueInput;

    public sealed class EmailAction : JobAction<QueueInput>
    {
        public static bool Executed { get; set; }

        public override Task<ActionResult> ExecuteAsync(
            QueueInput input,
            CancellationToken ct = default)
        {
            Executed = true;

            return Task.FromResult(ActionResults.None());
        }
    }

    public sealed class SmsAction : JobAction<QueueInput>
    {
        public static bool Executed { get; set; }

        public override Task<ActionResult> ExecuteAsync(
            QueueInput input,
            CancellationToken ct = default)
        {
            Executed = true;

            return Task.FromResult(ActionResults.None());
        }
    }
}