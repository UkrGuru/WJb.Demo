namespace BlazorWJb.Services;

public interface ILogFileReader
{
    Task<IEnumerable<string>> ReadLogLinesAsync(DateTime date, CancellationToken ct = default);
}

public class LogFileReader(IHostEnvironment env) : ILogFileReader
{
    private readonly IHostEnvironment _env = env;

    public async Task<IEnumerable<string>> ReadLogLinesAsync(
        DateTime date,
        CancellationToken ct = default)
    {
        var name = $"{date:yyyyMMdd}.log";
        var path = Path.Combine(_env.ContentRootPath, "Logs", name);

        if (!File.Exists(path))
            return Array.Empty<string>();

        var list = new List<string>(capacity: 4096);

        using var fs = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite,
            bufferSize: 4096,
            useAsync: true);

        using var sr = new StreamReader(fs);

        while (true)
        {
            ct.ThrowIfCancellationRequested();

            var line = await sr.ReadLineAsync().ConfigureAwait(false);
            if (line is null)
                break;

            list.Add(line);
        }

        return list;
    }
}

