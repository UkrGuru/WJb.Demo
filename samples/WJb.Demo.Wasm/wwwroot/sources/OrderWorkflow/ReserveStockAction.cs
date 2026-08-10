namespace WJb.Demo.Wasm.Actions;

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
