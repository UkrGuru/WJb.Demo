using WJb;

namespace WJbPro.Demos.Actions;

public sealed class HttpPingPayload
{
    public string? Url { get; set; }
}

public sealed class HttpPingAction : JobAction<HttpPingPayload>
{
    public const string Key = "http-ping";

    private readonly HttpClient _httpClient = new();

    public override async Task<IActionResult> ExecuteAsync(
        HttpPingPayload input, CancellationToken ct = default)
    {
        ReportProgress(10, $"HttpClient: {_httpClient != null}");

        var url = input.Url ?? "https://wjb.pro/";

        using var response = await _httpClient.GetAsync(url, ct);

        ReportProgress(100, $"{url} → {(int)response.StatusCode} {response.StatusCode}");

        return await CompleteAsync();
    }
}