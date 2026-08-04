# Progress

Progress allows an action to report execution status while it is running.

This is useful for:

- Long-running jobs
- Imports
- Exports
- Data migrations
- Batch processing

```text
Running Job
      ↓
Progress Updates
      ↓
Completed Job
```

---

## Updating Progress

Progress can be updated from an action.

```csharp
await Context.UpdateProgressAsync(
    25,
    "Reading file");
```

```csharp
await Context.UpdateProgressAsync(
    50,
    "Processing records");
```

```csharp
await Context.UpdateProgressAsync(
    100,
    "Completed");
```

---

## Progress Value

Progress uses a percentage value.

```csharp
0
```

Job started.

```csharp
100
```

Job completed.

Example:

```csharp
await Context.UpdateProgressAsync(
    75,
    "Uploading");
```

---

## Status Message

An optional message can be provided.

```csharp
await Context.UpdateProgressAsync(
    40,
    "Processing customers");
```

Stored values:

```text
Progress = 40

Message  = Processing customers
```

---

## Example

```csharp
public sealed class ImportAction : JobAction<ImportInput>
{
    public override async Task<ActionResult> ExecuteAsync(
        ImportInput input,
        CancellationToken ct)
    {
        await Context.UpdateProgressAsync(
            10,
            "Loading file");

        await LoadAsync(ct);

        await Context.UpdateProgressAsync(
            50,
            "Processing records");

        await ProcessAsync(ct);

        await Context.UpdateProgressAsync(
            100,
            "Completed");

        return ActionResults.None();
    }
}
```

---

## Monitoring

Progress information can be displayed by monitoring tools such as WJb.UI.Blazor.

Example:

```text
Import Customers

██████████░░░░░░░░░░ 50%

Processing records
```

---

## Completion

When a job completes successfully, WJb automatically sets progress to:

```text
100
```

if the current value is lower.

---

## Failure

If a job fails, the last progress value remains available for diagnostics.
