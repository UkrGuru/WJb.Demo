namespace WJb.DocTests;

public class _07_RetryTests
{
    [Fact]
    public void JobOptions_Should_Support_MaxRetries()
    {
        var options = new JobOptions
        {
            MaxRetries = 5
        };

        Assert.Equal(
            5,
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

    [Fact]
    public void RetryDelay_Should_Remain_Fixed_When_Backoff_Is_Disabled()
    {
        var options = new JobOptions
        {
            RetryDelay = TimeSpan.FromSeconds(10)
        };

        Assert.Equal(
            TimeSpan.FromSeconds(10),
            options.GetRetryDelay(0));

        Assert.Equal(
            TimeSpan.FromSeconds(10),
            options.GetRetryDelay(5));
    }

    [Fact]
    public void RetryDelay_Should_Double_When_Backoff_Is_Enabled()
    {
        var options = new JobOptions
        {
            RetryDelay = TimeSpan.FromMinutes(2),
            ExponentialBackoff = true
        };

        Assert.Equal(
            TimeSpan.FromMinutes(2),
            options.GetRetryDelay(0));

        Assert.Equal(
            TimeSpan.FromMinutes(4),
            options.GetRetryDelay(1));

        Assert.Equal(
            TimeSpan.FromMinutes(8),
            options.GetRetryDelay(2));

        Assert.Equal(
            TimeSpan.FromMinutes(16),
            options.GetRetryDelay(3));
    }

    [Fact]
    public void Retry_Attempt_Counter_Can_Be_Tracked_In_Payload()
    {
        var input = new EmailInput
        {
            To = "user@test.com",
            Attempt = 1
        };

        var next = new EmailInput
        {
            To = input.To,
            Attempt = input.Attempt + 1
        };

        Assert.Equal(2, next.Attempt);
    }

    [Fact]
    public void Retry_Should_Stop_When_Max_Attempts_Reached()
    {
        var input = new EmailInput
        {
            Attempt = 5
        };

        var shouldRetry =
            input.Attempt < 5;

        Assert.False(shouldRetry);
    }

    private sealed class EmailInput
    {
        public string To { get; init; } = "";

        public int Attempt { get; init; }
    }
}