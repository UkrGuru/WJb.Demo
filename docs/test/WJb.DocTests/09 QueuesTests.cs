namespace WJb.DocTests;

public class _09_QueuesTests
{
    [Fact]
    public void JobOptions_Should_Allow_Null_Queue()
    {
        var options = new JobOptions();

        Assert.Null(options.Queue);
    }

    [Fact]
    public void JobOptions_Should_Store_Queue_Name()
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
    public void JobOptions_Should_Support_Report_Queue()
    {
        var options = new JobOptions
        {
            Queue = "reports"
        };

        Assert.Equal(
            "reports",
            options.Queue);
    }

    [Fact]
    public void JobOptions_Should_Support_Background_Queue()
    {
        var options = new JobOptions
        {
            Queue = "background"
        };

        Assert.Equal(
            "background",
            options.Queue);
    }
}