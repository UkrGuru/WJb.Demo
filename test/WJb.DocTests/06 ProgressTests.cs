namespace WJb.DocTests;

public class _06_ProgressTests
{
    [Fact]
    public void JobProgress_Should_Have_Default_Values()
    {
        var progress = new JobProgress();

        Assert.Equal(0, progress.Progress);
        Assert.Null(progress.Message);
    }

    [Fact]
    public void JobProgress_Should_Support_Progress_Value()
    {
        var progress = new JobProgress
        {
            Progress = 75
        };

        Assert.Equal(75, progress.Progress);
    }

    [Fact]
    public void JobProgress_Should_Support_Message()
    {
        var progress = new JobProgress
        {
            Message = "Processing customers"
        };

        Assert.Equal(
            "Processing customers",
            progress.Message);
    }

    [Fact]
    public void JobProgress_Should_Support_Progress_And_Message()
    {
        var progress = new JobProgress
        {
            Progress = 40,
            Message = "Processing customers"
        };

        Assert.Equal(40, progress.Progress);
        Assert.Equal(
            "Processing customers",
            progress.Message);
    }

    [Fact]
    public void JobProgress_Should_Support_Completed_State()
    {
        var progress = new JobProgress
        {
            Progress = 100,
            Message = "Completed"
        };

        Assert.Equal(100, progress.Progress);
        Assert.Equal(
            "Completed",
            progress.Message);
    }
}