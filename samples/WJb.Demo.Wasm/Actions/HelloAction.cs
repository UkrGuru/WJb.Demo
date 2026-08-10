using WJb;

public sealed class HelloPayload
{
    public string? Text { get; set; }
}

public sealed class HelloAction : JobAction<HelloPayload>
{
    public const string Key = "hello";

    public override async Task<ActionResult> ExecuteAsync(HelloPayload input, CancellationToken ct = default)
    {
        var message = input.Text ?? "Hello from WJb ✅";

        ReportProgress(100, message);

        return ActionResults.None();
    }
}