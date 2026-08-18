namespace WJb.DocTests;

public class _02_ActionResultTests
{
    [Fact]
    public void Complete_Should_Return_CompleteResult()
    {
        var result = Results.Complete();

        Assert.IsType<CompleteResult>(
            result);
    }

    [Fact]
    public void Complete_Should_Accept_Anonymous_Object()
    {
        var result = Results.Complete(
            new
            {
                Sent = true,
                Count = 1
            });

        var complete =
            Assert.IsType<CompleteResult>(
                result);

        Assert.NotNull(
            complete.Value);
    }

    [Fact]
    public void Complete_Should_Accept_Int()
    {
        var result = Results.Complete(123);

        var complete =
            Assert.IsType<CompleteResult>(
                result);

        Assert.Equal(
            123,
            complete.Value);
    }

    [Fact]
    public void Complete_Should_Accept_String()
    {
        var result = Results.Complete("done");

        var complete =
            Assert.IsType<CompleteResult>(
                result);

        Assert.Equal(
            "done",
            complete.Value);
    }

    [Fact]
    public void Complete_Should_Accept_Boolean()
    {
        var result = Results.Complete(true);

        var complete =
            Assert.IsType<CompleteResult>(
                result);

        Assert.Equal(
            true,
            complete.Value);
    }

    [Fact]
    public void Next_Should_Accept_Single_Command()
    {
        var result = Results.Next(
            new JobCommand(
                "log",
                new LogInput
                {
                    Message = "Completed"
                }));

        var next =
            Assert.IsType<NextResult>(
                result);

        Assert.Single(
            next.Commands);
    }

    [Fact]
    public void Next_Should_Accept_Multiple_Commands()
    {
        var result = Results.Next(
            new JobCommand(
                "email",
                new EmailInput
                {
                    To = "user@test.com"
                }),
            new JobCommand(
                "audit",
                new AuditInput
                {
                    Event = "OrderCompleted"
                }));

        var next =
            Assert.IsType<NextResult>(
                result);

        Assert.Equal(
            2,
            next.Commands.Count);
    }

    [Fact]
    public void CompleteResult_Should_Support_Value()
    {
        var result = new CompleteResult(
            new
            {
                Success = true
            });

        Assert.NotNull(
            result.Value);
    }

    [Fact]
    public void NextResult_Should_Support_Commands()
    {
        var result =
            new NextResult(
                new JobCommand(
                    "audit",
                    new AuditInput()));

        Assert.Single(
            result.Commands);
    }

    private sealed class EmailInput
    {
        public string To { get; init; } = string.Empty;
    }

    private sealed class LogInput
    {
        public string Message { get; init; } = string.Empty;
    }

    private sealed class AuditInput
    {
        public string Event { get; init; } = string.Empty;
    }
}