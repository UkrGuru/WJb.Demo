namespace WJb.DocTests;

public class _05_ExecutorTests
{
    [Fact]
    public async Task Executor_Should_Execute_Action()
    {
        var action = new SendEmailAction();

        var result = await action.ExecuteAsync(
            new EmailInput
            {
                To = "user@test.com"
            },
            CancellationToken.None);

        Assert.IsType<CompleteResult>(result);
    }

    [Fact]
    public async Task Executor_Should_Store_Result()
    {
        var action = new ResultAction();

        var result = await action.ExecuteAsync(
            new EmailInput(),
            CancellationToken.None);

        var complete =
            Assert.IsType<CompleteResult>(result);

        Assert.NotNull(complete.Value);
    }

    [Fact]
    public async Task Executor_Should_Schedule_Single_Command()
    {
        var action = new NextAction();

        var result = await action.ExecuteAsync(
            new EmailInput(),
            CancellationToken.None);

        var next =
            Assert.IsType<NextResult>(result);

        Assert.Single(next.Commands);
    }

    [Fact]
    public async Task Executor_Should_Schedule_Multiple_Commands()
    {
        var action = new FanOutAction();

        var result = await action.ExecuteAsync(
            new EmailInput(),
            CancellationToken.None);

        var next =
            Assert.IsType<NextResult>(result);

        Assert.Equal(
            2,
            next.Commands.Count);
    }

    [Fact]
    public async Task Executor_Should_Propagate_Exception()
    {
        var action = new FailingAction();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => action.ExecuteAsync(
                new EmailInput(),
                CancellationToken.None));
    }

    [Fact]
    public async Task Executor_Should_Propagate_Cancellation()
    {
        var action = new CancellableAction();

        using var cts = new CancellationTokenSource();

        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => action.ExecuteAsync(
                new EmailInput(),
                cts.Token));
    }

    [ActionName("send-email")]
    private sealed class SendEmailAction
        : JobAction<EmailInput>
    {
        public override async Task<IActionResult> ExecuteAsync(
            EmailInput input,
            CancellationToken ct)
        {
            return await CompleteAsync();
        }
    }

    private sealed class ResultAction
        : JobAction<EmailInput>
    {
        public override async Task<IActionResult> ExecuteAsync(
            EmailInput input,
            CancellationToken ct)
        {
            return await CompleteAsync(
                new
                {
                    Success = true
                });
        }
    }

    private sealed class NextAction
        : JobAction<EmailInput>
    {
        public override async Task<IActionResult> ExecuteAsync(
            EmailInput input,
            CancellationToken ct)
        {
            return await NextAsync<AuditAction>(
                new AuditInput());
        }
    }

    private sealed class FanOutAction
        : JobAction<EmailInput>
    {
        public override async Task<IActionResult> ExecuteAsync(
            EmailInput input,
            CancellationToken ct)
        {
            return Results.Next(
                JobCommands.Next<EmailAction>(),
                JobCommands.Next<AuditAction>());
        }
    }

    private sealed class FailingAction
        : JobAction<EmailInput>
    {
        public override Task<IActionResult> ExecuteAsync(
            EmailInput input,
            CancellationToken ct)
        {
            throw new InvalidOperationException(
                "SMTP server unavailable");
        }
    }

    private sealed class CancellableAction
        : JobAction<EmailInput>
    {
        public override async Task<IActionResult> ExecuteAsync(
            EmailInput input,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            return await CompleteAsync();
        }
    }

    [ActionName("email")]
    private sealed class EmailAction
        : JobAction<EmailInput>
    {
        public override async Task<IActionResult> ExecuteAsync(
            EmailInput input,
            CancellationToken ct)
        {
            return await CompleteAsync();
        }
    }

    [ActionName("audit")]
    private sealed class AuditAction
        : JobAction<AuditInput>
    {
        public override async Task<IActionResult> ExecuteAsync(
            AuditInput input,
            CancellationToken ct)
        {
            return await CompleteAsync();
        }
    }

    private sealed class EmailInput
    {
        public string To { get; init; } = string.Empty;
    }

    private sealed class AuditInput
    {
    }
}