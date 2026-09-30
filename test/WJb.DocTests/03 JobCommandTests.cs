using WJb.Helpers;

namespace WJb.DocTests;

public class _03_StepTests
{
    [Fact]
    public void Step_Should_Store_Action()
    {
        var step = Steps.Next(
            "send-email",
            new EmailInput
            {
                To = "user@test.com"
            });

        Assert.Equal(
            "send-email",
            step.Action);
    }

    [Fact]
    public void Step_Should_Store_Typed_Payload()
    {
        var step = Steps.Next(
            "send-email",
            new EmailInput
            {
                To = "user@test.com"
            });

        var payload =
            JsonHelper.ToModel<EmailInput>(
                step.Payload);

        Assert.NotNull(payload);

        Assert.Equal(
            "user@test.com",
            payload!.To);
    }

    [Fact]
    public void Step_Should_Store_Anonymous_Payload()
    {
        var step = Steps.Next(
            "log",
            new
            {
                Message = "Completed"
            });

        var payload =
            step.Payload!.AsObject();

        Assert.NotNull(payload);

        Assert.Equal(
            "Completed",
            payload["message"]!
                .GetValue<string>());
    }

    [Fact]
    public void Next_Should_Add_Single_Step()
    {
        var result = Results.Done()
            .Next(
                "send-email",
                new EmailInput
                {
                    To = "user@test.com"
                });

        Assert.Single(
            result.Steps);
    }

    [Fact]
    public void Next_Should_Add_Multiple_Steps()
    {
        var result = Results.Done()
            .Next(
                "email",
                new EmailInput
                {
                    To = "user@test.com"
                })
            .Next(
                "audit",
                new AuditInput
                {
                    Event = "OrderCompleted"
                });

        Assert.Equal(
            2,
            result.Steps.Length);
    }

    [Fact]
    public void Steps_Next_Should_Create_Success_Step()
    {
        var step = Steps.Next(
            "send-email");

        Assert.Equal(
            StepCondition.Success,
            step.Condition);
    }

    [Fact]
    public void Steps_OnFailure_Should_Create_Failure_Step()
    {
        var step = Steps.OnFailure(
            "audit");

        Assert.Equal(
            StepCondition.Failure,
            step.Condition);
    }

    private sealed class EmailInput
    {
        public string To { get; init; } = string.Empty;
    }

    private sealed class AuditInput
    {
        public string Event { get; init; } = string.Empty;
    }
}