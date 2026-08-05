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

        Assert.False(result.Failed);
    }

    [Fact]
    public async Task Executor_Should_Store_Result()
    {
        var action = new ResultAction();

        var result = await action.ExecuteAsync(
            new EmailInput(),
            CancellationToken.None);

        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task Executor_Should_Schedule_Single_Command()
    {
        var action = new NextAction();

        var result = await action.ExecuteAsync(
            new EmailInput(),
            CancellationToken.None);

        Assert.Single(result.Commands);
    }

    [Fact]
    public async Task Executor_Should_Schedule_Multiple_Commands()
    {
        var action = new FanOutAction();

        var result = await action.ExecuteAsync(
            new EmailInput(),
            CancellationToken.None);

        Assert.Equal(
            2,
            result.Commands.Count());
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

    private sealed class SendEmailAction
        : JobAction<EmailInput>
    {
        public override Task<ActionResult> ExecuteAsync(
            EmailInput input,
            CancellationToken ct)
        {
            return Task.FromResult(
                ActionResults.None());
        }
    }

    private sealed class ResultAction
        : JobAction<EmailInput>
    {
        public override Task<ActionResult> ExecuteAsync(
            EmailInput input,
            CancellationToken ct)
        {
            return Task.FromResult(
                ActionResults.Result(
                    new
                    {
                        Success = true
                    }));
        }
    }

    private sealed class NextAction
        : JobAction<EmailInput>
    {
        public override Task<ActionResult> ExecuteAsync(
            EmailInput input,
            CancellationToken ct)
        {
            return Task.FromResult(
                ActionResults.Next(
                    new JobCommand(
                        "audit",
                        new AuditInput())));
        }
    }

    private sealed class FanOutAction
        : JobAction<EmailInput>
    {
        public override Task<ActionResult> ExecuteAsync(
            EmailInput input,
            CancellationToken ct)
        {
            return Task.FromResult(
                ActionResults.Next(
                    new JobCommand("email"),
                    new JobCommand("audit")));
        }
    }

    private sealed class FailingAction
        : JobAction<EmailInput>
    {
        public override Task<ActionResult> ExecuteAsync(
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
        public override Task<ActionResult> ExecuteAsync(
            EmailInput input,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            return Task.FromResult(
                ActionResults.None());
        }
    }

    private sealed class EmailInput
    {
        public string To { get; init; } = "";
    }

    private sealed class AuditInput
    {
    }
}