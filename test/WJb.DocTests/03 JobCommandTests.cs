namespace WJb.DocTests;

public class _03_JobCommandTests
{
    [Fact]
    public void JobCommand_Should_Store_Action()
    {
        var command = new JobCommand(
            "send-email",
            new EmailInput
            {
                To = "user@test.com"
            });

        Assert.Equal(
            "send-email",
            command.Action);
    }

    [Fact]
    public void JobCommand_Should_Store_Typed_Payload()
    {
        var command = new JobCommand(
            "send-email",
            new EmailInput
            {
                To = "user@test.com"
            });

        var payload =
            command.GetPayload<EmailInput>();

        Assert.NotNull(payload);

        Assert.Equal(
            "user@test.com",
            payload!.To);
    }

    [Fact]
    public void JobCommand_Should_Store_Anonymous_Payload()
    {
        var command = new JobCommand(
            "log",
            new
            {
                Message = "Completed"
            });

        var payload = command.AsObject();

        Assert.NotNull(payload);

        Assert.Equal(
            "Completed",
            payload!["Message"]!
                .GetValue<string>());
    }

    [Fact]
    public void Results_Next_Should_Accept_Single_Command()
    {
        var command = new JobCommand(
            "send-email",
            new EmailInput
            {
                To = "user@test.com"
            });

        var result =
            Results.Next(command);

        var next =
            Assert.IsType<NextResult>(
                result);

        Assert.Single(
            next.Commands);
    }

    [Fact]
    public void Results_Next_Should_Accept_Multiple_Commands()
    {
        var result =
            Results.Next(
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
    public void JobCommands_Next_Should_Create_Success_Command()
    {
        var command =
            JobCommands.Next(
                "send-email");

        Assert.Equal(
            JobCommandCondition.Success,
            command.Condition);
    }

    [Fact]
    public void JobCommands_OnFailure_Should_Create_Failure_Command()
    {
        var command =
            JobCommands.OnFailure(
                "audit");

        Assert.Equal(
            JobCommandCondition.Failure,
            command.Condition);
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