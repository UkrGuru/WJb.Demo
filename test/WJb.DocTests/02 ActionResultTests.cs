namespace WJb.DocTests;

public class _02_ActionResultTests
{
    [Fact]
    public void None_Should_Return_ActionResult()
    {
        var result = ActionResults.None();

        Assert.NotNull(result);
    }

    [Fact]
    public void Result_Should_Accept_Anonymous_Object()
    {
        var result = ActionResults.Result(
            new { Sent = true, Count = 1 });

        Assert.NotNull(result);
    }

    [Fact]
    public void Result_Should_Accept_Int()
    {
        var result = ActionResults.Result(123);

        Assert.NotNull(result);
    }

    [Fact]
    public void Result_Should_Accept_String()
    {
        var result = ActionResults.Result("done");

        Assert.NotNull(result);
    }

    [Fact]
    public void Result_Should_Accept_Boolean()
    {
        var result = ActionResults.Result(true);

        Assert.NotNull(result);
    }

    [Fact]
    public void Next_Should_Accept_Single_Command()
    {
        var result = ActionResults.Next(
            new JobCommand(
                "log", new LogInput { Message = "Completed" }));

        Assert.NotNull(result);
    }

    [Fact]
    public void Next_Should_Accept_Multiple_Commands()
    {
        var result = ActionResults.Next(
            new JobCommand(
                "email", new EmailInput { To = "user@test.com" }),
            new JobCommand(
                "audit",
                new AuditInput { Event = "OrderCompleted" }));

        Assert.NotNull(result);
    }

    [Fact]
    public void ActionResult_Should_Support_Value_And_Commands()
    {
        var result = new ActionResult
        {
            Value = new
            {
                Success = true
            },
            Commands =
            [
                new JobCommand(
                    "audit", new AuditInput())
            ]
        };

        Assert.NotNull(result.Value);
        Assert.Single(result.Commands);
    }

    private sealed class EmailInput
    {
        public string To { get; init; } = "";
    }

    private sealed class LogInput
    {
        public string Message { get; init; } = "";
    }

    private sealed class AuditInput
    {
        public string Event { get; init; } = "";
    }
}