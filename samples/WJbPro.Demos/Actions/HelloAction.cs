using WJb;

namespace WJbPro.Demos.Actions;

public sealed class HelloPayload
{
    public string? Text { get; set; }
}

public sealed class HelloAction : JobAction<HelloPayload>
{
    public const string Key = "hello";

    public override Task<IActionResult> ExecuteAsync(
        HelloPayload input, CancellationToken ct = default)
    {
        var message = input.Text ?? "Hello! ✅";

        ReportProgress(100, message);

        return CompleteAsync();
    }
}