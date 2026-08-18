namespace WJb.DocTests;

public class _01_ActionsTests
{
    [Fact]
    public async Task Action_Should_Receive_Strongly_Typed_Input()
    {
        var action = new CaptureEmailAction();

        await action.ExecuteAsync(
            new EmailInput
            {
                To = "user@test.com",
                Subject = "subject",
                Body = "body"
            });

        Assert.Equal("user@test.com", action.Input!.To);
        Assert.Equal("subject", action.Input.Subject);
        Assert.Equal("body", action.Input.Body);
    }

    [Fact]
    public async Task Action_Should_Return_Next_Command()
    {
        var action = new SendEmailAction();

        var result = await action.ExecuteAsync(
            new EmailInput
            {
                To = "user@test.com"
            });

        var next = Assert.IsType<NextResult>(result);

        Assert.Single(next.Commands);
    }

    [Fact]
    public async Task Action_Should_Return_Multiple_Next_Commands()
    {
        var action = new MultiStepAction();

        var result =
            await action.ExecuteAsync(
                new EmailInput());

        var next =
            Assert.IsType<NextResult>(result);

        Assert.Equal(2, next.Commands.Count);
    }

    [Fact]
    public async Task Workflow_Should_Execute_Email_Then_Log()
    {
        var emailAction = new SendEmailAction();

        var firstResult =
            await emailAction.ExecuteAsync(
                new EmailInput
                {
                    To = "user@test.com"
                });

        Assert.IsType<NextResult>(
            firstResult);

        var logAction = new LogAction();

        var secondResult =
            await logAction.ExecuteAsync(
                new LogInput
                {
                    Message =
                        "Email sent to user@test.com"
                });

        Assert.IsType<CompleteResult>(
            secondResult);
    }

    [Fact]
    public async Task Action_Should_Propagate_Exception()
    {
        var action = new FailingAction();

        await Assert.ThrowsAsync<
            InvalidOperationException>(
            () => action.ExecuteAsync(
                new EmailInput()));
    }

    private sealed class CaptureEmailAction
        : JobAction<EmailInput>
    {
        public EmailInput? Input { get; private set; }

        public override async Task<IActionResult>
            ExecuteAsync(
                EmailInput input,
                CancellationToken ct = default)
        {
            Input = input;

            return await CompleteAsync();
        }
    }

    [ActionName("send-email")]
    private sealed class SendEmailAction
        : JobAction<EmailInput>
    {
        public override async Task<IActionResult>
            ExecuteAsync(
                EmailInput input,
                CancellationToken ct = default)
        {
            return await NextAsync<LogAction>(
                new LogInput
                {
                    Message =
                        $"Email sent to {input.To}"
                });
        }
    }

    private sealed class MultiStepAction
        : JobAction<EmailInput>
    {
        public override async Task<IActionResult>
            ExecuteAsync(
                EmailInput input,
                CancellationToken ct = default)
        {
            return Results.Next(
                JobCommands.Next<LogAction>(
                    new LogInput
                    {
                        Message = "Email sent"
                    }),
                JobCommands.Next<AuditAction>(
                    new AuditInput
                    {
                        Event = "email"
                    }));
        }
    }

    [ActionName("log")]
    private sealed class LogAction
        : JobAction<LogInput>
    {
        public override async Task<IActionResult>
            ExecuteAsync(
                LogInput input,
                CancellationToken ct = default)
        {
            return await CompleteAsync();
        }
    }

    [ActionName("audit")]
    private sealed class AuditAction
        : JobAction<AuditInput>
    {
        public override async Task<IActionResult>
            ExecuteAsync(
                AuditInput input,
                CancellationToken ct = default)
        {
            return await CompleteAsync();
        }
    }

    private sealed class FailingAction
        : JobAction<EmailInput>
    {
        public override Task<IActionResult>
            ExecuteAsync(
                EmailInput input,
                CancellationToken ct = default)
        {
            throw new InvalidOperationException(
                "SMTP server unavailable");
        }
    }

    private sealed class EmailInput
    {
        public string To { get; init; } = string.Empty;

        public string Subject { get; init; } = string.Empty;

        public string Body { get; init; } = string.Empty;
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