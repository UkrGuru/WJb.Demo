using WJb;

namespace WJbPro.Demos.Actions;

public sealed class ImportCustomersInput
{
    public string Source { get; set; } = "CRM";
}

public sealed class ReportInput
{
    public int ImportedCustomers { get; set; }
}

public sealed class ImportCustomersAction
    : JobAction<ImportCustomersInput>, IProgressAction
{
    public const string Key = "import-customers";

    public override async ValueTask<ActionResult> ExecuteAsync(
        ImportCustomersInput? input,
        CancellationToken ct = default)
    {
        for (var i = 0; i <= 100; i += 25)
        {
            ReportProgress(
                i,
                $"Importing customers {i}%");

            await Task.Delay(500, ct);
        }

        return Results.Done()
            .Next(
                GenerateReportAction.Key,
                new ReportInput
                {
                    ImportedCustomers = 1250
                });
    }
}