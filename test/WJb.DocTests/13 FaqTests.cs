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
        var result = Results.Next(
            new JobCommand(
                "audit",
                new
                {
                    Id = 1
                }));

        var next =
            Assert.IsType<NextResult>(result);

        Assert.Single(next.Commands);
    }

    [Fact]
    public void Complete_Should_Support_Object_Result()
    {
        var result = Results.Complete(
            new
            {
                Success = true
            });

        var complete =
            Assert.IsType<CompleteResult>(result);

        Assert.NotNull(complete.Value);
    }

    [Fact]
    public void Complete_Should_Support_Integer_Result()
    {
        var result = Results.Complete(123);

        var complete =
            Assert.IsType<CompleteResult>(result);

        Assert.Equal(
            123,
            complete.Value);
    }

    [Fact]
    public void Complete_Should_Support_String_Result()
    {
        var result = Results.Complete("done");

        var complete =
            Assert.IsType<CompleteResult>(result);

        Assert.Equal(
            "done",
            complete.Value);
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

        Assert.True(
            options.ExponentialBackoff);
    }
}