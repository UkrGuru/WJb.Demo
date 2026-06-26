using System.Text;
using WJb;
using WJb.Contracts;

Console.OutputEncoding = Encoding.UTF8;

var factory = WJbConfig.Create(cfg =>
{
    cfg.AddAction<LogAction>();
});

var action = await factory.CreateAsync<LogAction>();

await action.ExecuteAsync(new { message = "Hello WJb! 👋" });

public class LogInput
{
    public string? Message { get; set; }
}

public class LogAction : JobAction<LogInput>
{
    // public const string Key = "myapp.log@v1";
    public override Task<ActionResult> ExecuteAsync(LogInput input, CancellationToken ct = default)
    {
        Console.WriteLine(input?.Message ?? "<no message>");
        return Task.FromResult(ActionResults.None());
    }
}


