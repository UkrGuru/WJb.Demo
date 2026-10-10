namespace WJb.DocTests;

public class _05_ExecutorTests
{
    [ActionName("send-email")]
    private sealed class SendEmailAction : JobAction<EmailInput>
    {
        public override ValueTask<ActionResult> ExecuteAsync(
            EmailInput? input, CancellationToken ct = default)
        {
            // business logic...

            return ActionResult.Empty;
        }
    }

    [Fact]
    public async Task Executor_Should_Execute_Action()
    {
        var action = new SendEmailAction();

        var result = await action.ExecuteAsync(
            new EmailInput { To = "user@test.com" }, CancellationToken.None);

        Assert.Null(result.Result);

        Assert.Empty(result.Steps);
    }

    [Fact]
    public async Task Executor_Should_Store_Result()
    {
        var action = new ResultAction();

        var result = await action.ExecuteAsync(
            new EmailInput(), CancellationToken.None);

        Assert.NotNull(result.Result);
    }

    [Fact]
    public async Task Executor_Should_Schedule_Single_Step()
    {
        var action = new NextAction();

        var result = await action.ExecuteAsync(
            new EmailInput(), CancellationToken.None);

        Assert.Single(result.Steps);
    }

    [Fact]
    public async Task Executor_Should_Schedule_Multiple_Steps()
    {
        var action = new FanOutAction();

        var result = await action.ExecuteAsync(
            new EmailInput(), CancellationToken.None);

        Assert.Equal(2, result.Steps.Length);
    }

    [Fact]
    public async Task Executor_Should_Propagate_Exception()
    {
        var action = new FailingAction();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => action.ExecuteAsync(
                    new EmailInput(), CancellationToken.None)
                .AsTask());
    }

    [Fact]
    public async Task Executor_Should_Propagate_Cancellation()
    {
        var action = new CancellableAction();

        using var cts = new CancellationTokenSource();

        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => action.ExecuteAsync(
                    new EmailInput(), cts.Token)
                .AsTask());
    }


    private sealed class ResultAction : JobAction<EmailInput>
    {
        public override ValueTask<ActionResult> ExecuteAsync(EmailInput? input, CancellationToken ct = default)
            => new ActionResult(new { Success = true });
    }

    private sealed class NextAction : JobAction<EmailInput>
    {
        public override ValueTask<ActionResult> ExecuteAsync(EmailInput? input, CancellationToken ct = default)
            => new ActionResult()
                .Next<AuditAction>(new AuditInput());
    }

    private sealed class FanOutAction : JobAction<EmailInput>
    {
        public override ValueTask<ActionResult> ExecuteAsync(EmailInput? input, CancellationToken ct = default)
            => new ActionResult()
                .Next<EmailAction>()
                .Next<AuditAction>();
    }

    private sealed class FailingAction : JobAction<EmailInput>
    {
        public override ValueTask<ActionResult> ExecuteAsync(EmailInput? input, CancellationToken ct = default)
            => throw new InvalidOperationException("SMTP server unavailable");
    }

    private sealed class CancellableAction : JobAction<EmailInput>
    {
        public override ValueTask<ActionResult> ExecuteAsync(EmailInput? input, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            return ActionResult.Empty;
        }
    }

    [ActionName("email")]
    private sealed class EmailAction : JobAction<EmailInput>
    {
        public override ValueTask<ActionResult> ExecuteAsync(EmailInput? input, CancellationToken ct = default)
            => ActionResult.Empty;
    }

    [ActionName("audit")]
    private sealed class AuditAction : JobAction<AuditInput>
    {
        public override ValueTask<ActionResult> ExecuteAsync(AuditInput? input, CancellationToken ct = default)
            => ActionResult.Empty;
    }

    private sealed class EmailInput
    {
        public string To { get; init; } = string.Empty;
    }

    private sealed class AuditInput
    {
    }
}