namespace WJb.DocTests;

public class _08_SchedulingTests
{
    [Fact]
    public void Delay_Should_Schedule_Job_In_Future()
    {
        var now = new DateTime(2026, 1, 1);

        var options = new JobOptions
        {
            Delay = TimeSpan.FromMinutes(10)
        };

        Assert.Equal(
            now.AddMinutes(10),
            options.GetRunAt(now));
    }

    [Fact]
    public void Zero_Delay_Should_Not_Create_Schedule()
    {
        var options = new JobOptions();

        Assert.Null(
            options.GetRunAt(DateTime.UtcNow));
    }

    [Fact]
    public void Negative_Delay_Should_Not_Create_Schedule()
    {
        var options = new JobOptions
        {
            Delay = TimeSpan.FromMinutes(-1)
        };

        Assert.Null(
            options.GetRunAt(DateTime.UtcNow));
    }

    [Fact]
    public void JobOptions_Should_Support_Minute_Delay()
    {
        var options = new JobOptions
        {
            Delay = TimeSpan.FromMinutes(10)
        };

        Assert.Equal(
            TimeSpan.FromMinutes(10),
            options.Delay);
    }

    [Fact]
    public void JobOptions_Should_Support_Day_Delay()
    {
        var options = new JobOptions
        {
            Delay = TimeSpan.FromDays(7)
        };

        Assert.Equal(
            TimeSpan.FromDays(7),
            options.Delay);
    }

    [Fact]
    public void Scheduling_And_Queue_Can_Be_Combined()
    {
        var options = new JobOptions
        {
            Delay = TimeSpan.FromMinutes(30),
            Queue = "email"
        };

        Assert.Equal(
            TimeSpan.FromMinutes(30),
            options.Delay);

        Assert.Equal(
            "email",
            options.Queue);
    }
}