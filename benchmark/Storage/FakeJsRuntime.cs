using Microsoft.JSInterop;

internal sealed class FakeJsRuntime : IJSRuntime
{
    private readonly Dictionary<string, object?> _data = [];

    public ValueTask<TValue> InvokeAsync<TValue>(
        string identifier,
        object?[]? args)
        => InvokeAsync<TValue>(
            identifier,
            CancellationToken.None,
            args);

    public ValueTask<TValue> InvokeAsync<TValue>(
        string identifier,
        CancellationToken cancellationToken,
        object?[]? args)
    {
        object? result = null;

        switch (identifier)
        {
            case "wjbIdb.set":
                {
                    var store = (string)args![0]!;
                    var key = (string)args[1]!;

                    _data[$"{store}:{key}"] = args[2];
                    break;
                }

            case "wjbIdb.get":
                {
                    var store = (string)args![0]!;
                    var key = (string)args[1]!;

                    _data.TryGetValue(
                        $"{store}:{key}",
                        out result);

                    break;
                }

            case "wjbIdb.remove":
                {
                    var store = (string)args![0]!;
                    var key = (string)args[1]!;

                    result = _data.Remove(
                        $"{store}:{key}");

                    break;
                }

            case "wjbIdb.exists":
                {
                    var store = (string)args![0]!;
                    var key = (string)args[1]!;

                    result = _data.ContainsKey(
                        $"{store}:{key}");

                    break;
                }

            case "wjbIdb.keys":
                {
                    var store = (string)args![0]!;

                    result = _data.Keys
                        .Where(x => x.StartsWith(
                            store + ":",
                            StringComparison.Ordinal))
                        .Select(x => x[(store.Length + 1)..])
                        .ToList();

                    break;
                }
        }

        return ValueTask.FromResult(
            (TValue?)result!);
    }
}