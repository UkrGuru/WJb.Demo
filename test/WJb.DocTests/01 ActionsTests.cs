namespace WJb.DocTests;

public class _01_ActionsTests
{
    [Fact]
    public async Task Action_Should_Receive_Strongly_Typed_Input()
    {
        var action = new CaptureEmailAction();

        await action.ExecuteAsync(
            new EmailInput { To = "user@test.com", Subject = "subject", Body = "body" });

        Assert.Equal("user@test.com", action.Input!.To);
        Assert.Equal("subject", action.Input.Subject);
        Assert.Equal("body", action.Input.Body);
    }

    [Fact]
    public async Task Action_Should_Return_Next_Command()
    {
        var action = new SendEmailAction();

        var result = await action.ExecuteAsync(
            new EmailInput { To = "user@test.com" });

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Action_Should_Return_Multiple_Next_Commands()
    {
        var action = new MultiStepAction();

        var result = await action.ExecuteAsync(new EmailInput());

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Workflow_Should_Execute_Email_Then_Log()
    {
        var emailAction = new SendEmailAction();

        var firstResult = await emailAction.ExecuteAsync(
            new EmailInput { To = "user@test.com" });

        Assert.NotNull(firstResult);

        var logAction = new LogAction();

        var secondResult = await logAction.ExecuteAsync(
            new LogInput { Message = "Email sent to user@test.com" });

        Assert.NotNull(secondResult);
    }

    [Fact]
    public async Task Action_Should_Propagate_Exception()
    {
        var action = new FailingAction();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => action.ExecuteAsync(new EmailInput()));
    }

    private sealed class CaptureEmailAction : JobAction<EmailInput>
    {
        public EmailInput? Input { get; private set; }

        public override Task<ActionResult> ExecuteAsync(
            EmailInput input, CancellationToken ct = default)
        {
            Input = input;

            return Task.FromResult(ActionResults.None());
        }
    }

    private sealed class SendEmailAction : JobAction<EmailInput>
    {
        public override Task<ActionResult> ExecuteAsync(
            EmailInput input, CancellationToken ct = default)
        {
            return Task.FromResult(ActionResults.Next(
                new JobCommand("log", new LogInput { Message = $"Email sent to {input.To}" })));
        }
    }

    private sealed class MultiStepAction : JobAction<EmailInput>
    {
        public override Task<ActionResult> ExecuteAsync(
            EmailInput input, CancellationToken ct = default)
        {
            return Task.FromResult(
                ActionResults.Next(
                    new JobCommand(
                        "log", new LogInput { Message = "Email sent" }),
                    new JobCommand(
                        "audit", new AuditInput { Event = "email" })));
        }
    }

    private sealed class LogAction : JobAction<LogInput>
    {
        public override Task<ActionResult> ExecuteAsync(
            LogInput input, CancellationToken ct = default)
        {
            return Task.FromResult(ActionResults.None());
        }
    }

    private sealed class FailingAction : JobAction<EmailInput>
    {
        public override Task<ActionResult> ExecuteAsync(
            EmailInput input, CancellationToken ct = default)
        {
            throw new InvalidOperationException("SMTP server unavailable");
        }
    }

    private sealed class EmailInput
    {
        public string To { get; init; } = "";

        public string Subject { get; init; } = "";

        public string Body { get; init; } = "";
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