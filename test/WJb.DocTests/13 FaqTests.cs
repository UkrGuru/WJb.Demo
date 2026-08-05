namespace WJb.DocTests;

public class _13_FaqTests
{
    [Fact]
    public void JobOptions_Should_Support_Delay()
    {
        var options = new JobOptions
        {
            Delay = TimeSpan.FromMinutes(5)
        };

        Assert.Equal(
            TimeSpan.FromMinutes(5),
            options.Delay);
    }

    [Fact]
    public void JobOptions_Should_Support_Queue()
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
    public void JobCommand_Should_Schedule_Next_Work()
    {
        var result = ActionResults.Next(
            new JobCommand(
                "audit",
                new
                {
                    Id = 1
                }));

        Assert.Single(result.Commands);
    }

    [Fact]
    public void ActionResults_Should_Support_Object_Result()
    {
        var result = ActionResults.Result(
            new
            {
                Success = true
            });

        Assert.NotNull(result.Value);
    }

    [Fact]
    public void ActionResults_Should_Support_Integer_Result()
    {
        var result = ActionResults.Result(123);

        Assert.Equal(
            123,
            result.Value);
    }

    [Fact]
    public void ActionResults_Should_Support_String_Result()
    {
        var result = ActionResults.Result("done");

        Assert.Equal(
            "done",
            result.Value);
    }

    [Fact]
    public void JobOptions_Should_Support_Retries()
    {
        var options = new JobOptions
        {
            MaxRetries = 3
        };

        Assert.Equal(
            3,
            options.MaxRetries);
    }

    [Fact]
    public void JobOptions_Should_Support_RetryDelay()
    {
        var options = new JobOptions
        {
            RetryDelay = TimeSpan.FromMinutes(1)
        };

        Assert.Equal(
            TimeSpan.FromMinutes(1),
            options.RetryDelay);
    }

    [Fact]
    public void JobOptions_Should_Support_ExponentialBackoff()
    {
        var options = new JobOptions
        {
            ExponentialBackoff = true
        };

        Assert.True(options.ExponentialBackoff);
    }
}