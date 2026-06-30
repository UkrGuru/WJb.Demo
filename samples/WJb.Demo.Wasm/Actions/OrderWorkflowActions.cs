using WJb.Contracts;

namespace WJb.Demo.Wasm.Actions;

public sealed class OrderInput
{
    public int OrderId { get; set; }
}

public sealed class CreateOrderAction
    : JobAction<OrderInput>
{
    public override async Task<ActionResult> ExecuteAsync(
        OrderInput input,
        CancellationToken ct)
    {
        ReportProgress(25, "Order created");

        await Task.Delay(300, ct);

        return ActionResults.Next(
            JobCommands.Next<ReserveStockAction>(
                input));
    }
}

public sealed class ReserveStockAction
    : JobAction<OrderInput>
{
    public override async Task<ActionResult> ExecuteAsync(
        OrderInput input,
        CancellationToken ct)
    {
        ReportProgress(50, "Stock reserved");

        await Task.Delay(300, ct);

        return ActionResults.Next(
            JobCommands.Next<ChargePaymentAction>(
                input));
    }
}

public sealed class ChargePaymentAction
    : JobAction<OrderInput>
{
    public override async Task<ActionResult> ExecuteAsync(
        OrderInput input,
        CancellationToken ct)
    {
        ReportProgress(75, "Payment charged");

        await Task.Delay(300, ct);

        return ActionResults.Next(
            JobCommands.Next<SendConfirmationAction>(
                input));
    }
}

public sealed class SendConfirmationAction
    : JobAction<OrderInput>
{
    public override async Task<ActionResult> ExecuteAsync(
        OrderInput input,
        CancellationToken ct)
    {
        ReportProgress(
            100,
            $"Order #{input.OrderId} completed");

        await Task.Delay(300, ct);

        return ActionResults.Next(
            JobCommands.Next<LogAction>(
                new LogInput
                {
                    Message = $"Workflow completed for order #{input.OrderId}"
                }));
    }
}