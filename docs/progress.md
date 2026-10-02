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

Progress can be updated from an action using `ReportProgress`.

```csharp
ReportProgress(25, "Reading file");
```

```csharp
ReportProgress(50, "Processing records");
```

```csharp
ReportProgress(100, "Completed");
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
ReportProgress(75, "Uploading");
```

---

## Status Message

An optional message can be provided.

```csharp
ReportProgress(40, "Processing customers");
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
    public override async ValueTask<ActionResult> ExecuteAsync(
        ImportInput? input,
        CancellationToken ct = default)
    {
        ReportProgress(10, "Loading file");

        await LoadAsync(ct);

        ReportProgress(50, "Processing records");

        await ProcessAsync(ct);

        ReportProgress(100, "Completed");

        return Results.Done();
    }
}
```

---

## Monitoring

Progress information can be displayed by monitoring tools such as **WJb.UI.Blazor**.

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

---

## IProgressAction

Actions that need progress reporting typically implement or inherit behavior related to `IProgressAction`.

Most applications simply call `ReportProgress(...)` from within a `JobAction<T>`.

---

## Source Code

Documentation examples are verified by automated documentation tests.

Tests:

```text
../test/WJb.DocTests/06_ProgressTests.cs
```
