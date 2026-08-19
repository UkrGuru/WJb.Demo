public sealed class ChargePaymentAction : JobAction<OrderInput>
{
    public const string Key = "charge-payment";

    public override async Task<IActionResult> ExecuteAsync(
        OrderInput input, CancellationToken ct)
    {
        ReportProgress(75, "Payment charged");

        await Task.Delay(300, ct);

        return Results.Next(new JobCommand(SendConfirmationAction.Key, input));
    }
}