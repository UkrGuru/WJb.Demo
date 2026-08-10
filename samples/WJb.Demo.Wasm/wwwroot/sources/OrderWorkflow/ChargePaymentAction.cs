namespace WJb.Demo.Wasm.Actions;

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