using WJb;

public sealed class ReserveStockAction : JobAction<OrderInput>
{
    public const string Key = "reserve-stock";

    public override async Task<IActionResult> ExecuteAsync(
        OrderInput input, CancellationToken ct)
    {
        ReportProgress(50, "Stock reserved");

        await Task.Delay(300, ct);

        return Results.Next(new JobCommand(ChargePaymentAction.Key, input));
    }
}