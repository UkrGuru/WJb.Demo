using WJb;

namespace WJbPro.Demos.Actions;

public sealed class OrderInput
{
    public int OrderId { get; set; }
}

public sealed class CreateOrderAction : JobAction<OrderInput>
{
    public const string Key = "create-order";

    public override async Task<IActionResult> ExecuteAsync(
        OrderInput input, CancellationToken ct)
    {
        ReportProgress(25, "Order created");

        await Task.Delay(300, ct);

        return Results.Next(new JobCommand(ReserveStockAction.Key, input));
    }
}