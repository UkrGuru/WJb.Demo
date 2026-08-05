namespace WJb.DocTests;

public class _12_WjbSqlTests
{
    [Fact]
    public void JobOptions_Should_Support_Scheduled_Jobs()
    {
        var options = new JobOptions
        {
            Delay = TimeSpan.FromHours(1)
        };

        Assert.Equal(
            TimeSpan.FromHours(1),
            options.Delay);
    }

    [Fact]
    public void JobOptions_Should_Support_Queues()
    {
        var options = new JobOptions
        {
            Queue = "email"
        };

        Assert.Equal(
            "email",
            options.Queue);
    }

    [Fact]
    public void JobCommand_Should_Support_Email_Payload()
    {
        var command = new JobCommand(
            "send-email",
            new
            {
                To = "user@test.com"
            });

        var payload = command.AsObject();

        Assert.NotNull(payload);
    }

    [Fact]
    public void ActionResults_Result_Should_Support_Integer_Result()
    {
        var result = ActionResults.Result(123);

        Assert.Equal(
            123,
            result.Value);
    }

    [Fact]
    public void ActionResults_Result_Should_Support_String_Result()
    {
        var result = ActionResults.Result("Done");

        Assert.Equal(
            "Done",
            result.Value);
    }

    [Fact]
    public void ActionResults_Result_Should_Support_Object_Result()
    {
        var result = ActionResults.Result(
            new
            {
                Sent = true
            });

        Assert.NotNull(result.Value);
    }

    [Fact]
    public void JobProgress_Should_Support_Progress_Tracking()
    {
        var progress = new JobProgress
        {
            Progress = 50,
            Message = "Processing records"
        };

        Assert.Equal(50, progress.Progress);
        Assert.Equal(
            "Processing records",
            progress.Message);
    }

    [Fact]
    public void ActionResults_Result_Should_Support_Boolean_Result()
    {
        var result = ActionResults.Result(true);

        Assert.Equal(
            true,
            result.Value);
    }
}