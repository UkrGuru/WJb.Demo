using WJb;

public sealed class SendConfirmationAction : JobAction<OrderInput>
{
    public const string Key = "send-confirmation";

    public override async Task<IActionResult> ExecuteAsync(
        OrderInput input, CancellationToken ct)
    {
        ReportProgress(100, $"Order #{input.OrderId} completed");

        await Task.Delay(300, ct);

        return Results.Next(new JobCommand(LogAction.Key,
                new LogInput { Message = $"Workflow completed for order #{input.OrderId}" }));
    }
}