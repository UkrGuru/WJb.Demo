namespace WJb.Demo.Wasm.Actions;

public sealed class OrderInput
{
    public int OrderId { get; set; }
}

public sealed class CreateOrderAction : JobAction<OrderInput>
{
    public const string Key = "create-order";

    public override async Task<ActionResult> ExecuteAsync(OrderInput input, CancellationToken ct)
    {
        ReportProgress(25, "Order created");

        await Task.Delay(300, ct);

        return ActionResults.Next(new JobCommand(ReserveStockAction.Key, input));
    }
}

public sealed class ReserveStockAction : JobAction<OrderInput>
{
    public const string Key = "reserve-stock";

    public override async Task<ActionResult> ExecuteAsync(OrderInput input, CancellationToken ct)
    {
        ReportProgress(50, "Stock reserved");

        await Task.Delay(300, ct);

        return ActionResults.Next(new JobCommand(ChargePaymentAction.Key, input));
    }
}

public sealed class ChargePaymentAction : JobAction<OrderInput>
{
    public const string Key = "charge-payment";

    public override async Task<ActionResult> ExecuteAsync(OrderInput input, CancellationToken ct)
    {
        ReportProgress(75, "Payment charged");

        await Task.Delay(300, ct);

        return ActionResults.Next(new JobCommand(SendConfirmationAction.Key, input));
    }
}

public sealed class SendConfirmationAction : JobAction<OrderInput>
{
    public const string Key = "send-confirmation";

    public override async Task<ActionResult> ExecuteAsync(OrderInput input, CancellationToken ct)
    {
        ReportProgress(100, $"Order #{input.OrderId} completed");

        await Task.Delay(300, ct);

        return ActionResults.Next(new JobCommand(
            LogAction.Key,
            new LogInput
            {
                Message = $"Workflow completed for order #{input.OrderId}"
            }));
    }
}