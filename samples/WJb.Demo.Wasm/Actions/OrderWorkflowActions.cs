namespace WJb.Demo.Wasm.Actions;

public sealed class OrderInput
{
    public int OrderId { get; set; }
}

[ActionName("create-order")]
public sealed class CreateOrderAction
    : JobAction<OrderInput>
{
    public override async Task<IActionResult> ExecuteAsync(
        OrderInput input,
        CancellationToken ct)
    {
        ReportProgress(
            25,
            "Order created");

        await Task.Delay(
            300,
            ct);

        return await NextAsync<ReserveStockAction>(
            input);
    }
}

[ActionName("reserve-stock")]
public sealed class ReserveStockAction
    : JobAction<OrderInput>
{
    public override async Task<IActionResult> ExecuteAsync(
        OrderInput input,
        CancellationToken ct)
    {
        ReportProgress(
            50,
            "Stock reserved");

        await Task.Delay(
            300,
            ct);

        return await NextAsync<ChargePaymentAction>(
            input);
    }
}

[ActionName("charge-payment")]
public sealed class ChargePaymentAction
    : JobAction<OrderInput>
{
    public override async Task<IActionResult> ExecuteAsync(
        OrderInput input,
        CancellationToken ct)
    {
        ReportProgress(
            75,
            "Payment charged");

        await Task.Delay(
            300,
            ct);

        return await NextAsync<SendConfirmationAction>(
            input);
    }
}

[ActionName("send-confirmation")]
public sealed class SendConfirmationAction
    : JobAction<OrderInput>
{
    public override async Task<IActionResult> ExecuteAsync(
        OrderInput input,
        CancellationToken ct)
    {
        ReportProgress(
            100,
            $"Order #{input.OrderId} completed");

        await Task.Delay(
            300,
            ct);

        return await NextAsync<LogAction>(
            new LogInput
            {
                Message =
                    $"Workflow completed for order #{input.OrderId}"
            });
    }
}