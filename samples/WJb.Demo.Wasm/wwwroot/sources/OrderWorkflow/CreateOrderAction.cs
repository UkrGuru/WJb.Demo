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