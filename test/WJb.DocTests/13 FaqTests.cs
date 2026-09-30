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
    public void ActionResult_Should_Schedule_Next_Work()
    {
        var result = Results.Done()
            .Next(
                "audit",
                new
                {
                    Id = 1
                });

        Assert.Single(result.Steps);
    }

    [Fact]
    public void Done_Should_Support_Object_Result()
    {
        var result = Results.Done(
            new
            {
                Success = true
            });

        Assert.NotNull(result.Result);
    }

    [Fact]
    public void Done_Should_Support_Integer_Result()
    {
        var result = Results.Done(123);

        Assert.Equal(
            123,
            result.Result!
                .GetValue<int>());
    }

    [Fact]
    public void Done_Should_Support_String_Result()
    {
        var result = Results.Done("done");

        Assert.Equal(
            "done",
            result.Result!
                .GetValue<string>());
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