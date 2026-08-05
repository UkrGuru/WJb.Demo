namespace WJb.DocTests;

public class _04_JobOptionsTests
{
    [Fact]
    public void JobOptions_Should_Have_Default_Values()
    {
        var options = new JobOptions();

        Assert.Null(options.Queue);
        Assert.Equal(TimeSpan.Zero, options.Delay);
        Assert.Equal(0, options.MaxRetries);
        Assert.Equal(TimeSpan.FromSeconds(5), options.RetryDelay);
        Assert.False(options.ExponentialBackoff);
    }

    [Fact]
    public void JobOptions_Should_Accept_Delay()
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
    public void JobOptions_Should_Accept_Queue()
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
    public void JobOptions_Should_Accept_MaxRetries()
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
    public void JobOptions_Should_Accept_RetryDelay()
    {
        var options = new JobOptions
        {
            RetryDelay = TimeSpan.FromSeconds(10)
        };

        Assert.Equal(
            TimeSpan.FromSeconds(10),
            options.RetryDelay);
    }

    [Fact]
    public void JobOptions_Should_Accept_ExponentialBackoff()
    {
        var options = new JobOptions
        {
            ExponentialBackoff = true
        };

        Assert.True(options.ExponentialBackoff);
    }

    [Fact]
    public void Delay_Should_Calculate_RunAt()
    {
        var now = new DateTime(2026, 1, 1);

        var options = new JobOptions
        {
            Delay = TimeSpan.FromMinutes(5)
        };

        var runAt = options.GetRunAt(now);

        Assert.Equal(
            now.AddMinutes(5),
            runAt);
    }

    [Fact]
    public void ExponentialBackoff_Should_Increase_Retry_Delay()
    {
        var options = new JobOptions
        {
            RetryDelay = TimeSpan.FromSeconds(5),
            ExponentialBackoff = true
        };

        Assert.Equal(
            TimeSpan.FromSeconds(5),
            options.GetRetryDelay(0));

        Assert.Equal(
            TimeSpan.FromSeconds(10),
            options.GetRetryDelay(1));

        Assert.Equal(
            TimeSpan.FromSeconds(20),
            options.GetRetryDelay(2));
    }
}