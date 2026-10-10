namespace WJb.DocTests;

public class _02_ActionResultTests
{
    [Fact]
    public void Done_Should_Return_ActionResult()
    {
        var result = ActionResult.Empty;

        Assert.NotNull(result);
    }

    [Fact]
    public void Done_Should_Accept_Anonymous_Object()
    {
        var result = new ActionResult(
            new
            {
                Sent = true,
                Count = 1
            });

        Assert.NotNull(result.Result);
    }

    [Fact]
    public void Done_Should_Accept_Int()
    {
        var result = new ActionResult(123);

        Assert.Equal(123, result.Result!.GetValue<int>());
    }

    [Fact]
    public void Done_Should_Accept_String()
    {
        var result = new ActionResult("done");

        Assert.Equal("done", result.Result!.GetValue<string>());
    }

    [Fact]
    public void Done_Should_Accept_Boolean()
    {
        var result = new ActionResult(true);

        Assert.True(result.Result!.GetValue<bool>());
    }

    [Fact]
    public void Next_Should_Add_Single_Step()
    {
        var result = new ActionResult()
            .Next("log", new LogInput { Message = "Completed" });

        Assert.Single(result.Steps);
    }

    [Fact]
    public void Next_Should_Add_Multiple_Steps()
    {
        var result = new ActionResult()
            .Next("email", new EmailInput { To = "user@test.com" })
            .Next("audit", new AuditInput { Event = "OrderCompleted" });

        Assert.Equal(2, result.Steps.Length);
    }

    [Fact]
    public void ActionResult_Should_Support_Result()
    {
        var result = new ActionResult(new { Success = true });

        Assert.NotNull(result.Result);
    }

    [Fact]
    public void ActionResult_Should_Support_Steps()
    {
        var result = new ActionResult()
            .Next("audit", new AuditInput());

        Assert.Single(result.Steps);
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