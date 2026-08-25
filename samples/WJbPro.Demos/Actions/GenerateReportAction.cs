using WJb;

namespace WJbPro.Demos.Actions;

public sealed class GenerateReportAction
    : JobAction<ReportInput>, IProgressAction
{
    public const string Key = "generate-report";

    public override async Task<IActionResult> ExecuteAsync(
        ReportInput input, CancellationToken ct)
    {
        ReportProgress(25, "Preparing report");

        await Task.Delay(500, ct);

        ReportProgress(75, "Finalizing report");

        await Task.Delay(500, ct);

        ReportProgress(100, "Report generated");

        return Results.Next(new JobCommand(SendEmailAction.Key,
                new EmailInput
                {
                    To = "admin@demo.local",
                    Subject = $"Imported {input.ImportedCustomers} customers"
                }));
    }
}