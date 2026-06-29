using WJb.Contracts;

namespace WJb.Demo.Wasm.Actions;

public sealed class HelloPayload
{
    public string? Text { get; set; }
}

public sealed class HelloAction : JobAction<HelloPayload>
{
    public override Task<ActionResult> ExecuteAsync(
        HelloPayload input,
        CancellationToken ct = default)
    {
        var message = input.Text ?? "Hello from WJb ✅";

        ReportProgress(100, message);

        return Task.FromResult(ActionResults.None());
    }
}