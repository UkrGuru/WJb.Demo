# 🚀 WJb Minimal API Demo

A minimal ASP.NET Core API demonstrating background job execution with **WJb**.

## Features

- Job enqueueing via HTTP
- Background processing with `WasmWorker`
- Progress reporting
- In-memory storage
- Job querying and deletion

## Architecture

```text
POST /jobs
    ↓
IWJb.EnqueueAsync()
    ↓
InMemoryStore
    ↓
WasmWorker
    ↓
DemoAction
    ↓
Completed Job
```

## Configuration

```csharp
builder.Services.AddSingleton<IStore, InMemoryStore>();

builder.Services.AddSingleton<IWJb>(sp =>
{
    var store = sp.GetRequiredService<IStore>();

    return WJbBuilder.Create(store, cfg =>
    {
        cfg.AddAction<DemoAction>(DemoAction.Key);
    });
});

builder.Services.AddSingleton(sp =>
    new WasmWorker(sp.GetRequiredService<IWJb>()));
```

## Run

```bash
dotnet run
```

## API

### Create Job

```http
POST /jobs
```

Response:

```json
{
  "jobId": "019f37e6-a40f-7639-8a24-d77bf860647a"
}
```

### List Jobs

```http
GET /jobs
```

### Get Job

```http
GET /jobs/{id}
```

### Delete Job

```http
DELETE /jobs/{id}
```

## Demo Action

```csharp
public sealed class DemoAction : JobAction<DemoPayload>, IProgressAction
{
    public const string Key = "demo";

    public override async Task<IActionResult> ExecuteAsync(
        DemoPayload input, CancellationToken ct = default)
    {
        for (var i = 0; i <= 100; i += 10)
        {
            await Task.Delay(input.DelayMs / 10, ct);
            ReportProgress(i, $"Progress {i}%");
        }

        return await CompleteAsync("Done ✅");
    }
}
```

## Payload

```json
{
  "delayMs": 5000,
  "text": "Done ✅"
}
```

## Learn More

- https://www.nuget.org/packages?q=WJb
- https://wjb.pro
- https://github.com/UkrGuru/WJb.Demo
