namespace WJb.Demo.Monitor;

public static class Actions
{
    public const string ImportCustomers = "import-customers";

    public const string GenerateReport = "generate-report";

    public const string SendEmail = "send-email";
}

public sealed class ImportCustomersInput
{
    public string Source { get; set; } = "CRM";
}

public sealed class ReportInput
{
    public int ImportedCustomers { get; set; }
}

[ActionName(Actions.ImportCustomers)]
public sealed class ImportCustomersAction
    : JobAction<ImportCustomersInput>, IProgressAction
{
    public override async Task<IActionResult> ExecuteAsync(
        ImportCustomersInput input,        CancellationToken ct)
    {
        for (var i = 0; i <= 100; i += 25)
        {
            ReportProgress(i, $"Importing customers {i}%");

            await Task.Delay(500, ct);
        }

        return await NextAsync<GenerateReportAction>(
            new ReportInput
            {
                ImportedCustomers = 1250
            });
    }
}

public sealed class EmailInput
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
}

[ActionName(Actions.GenerateReport)]
public sealed class GenerateReportAction
    : JobAction<ReportInput>, IProgressAction
{
    public override async Task<IActionResult> ExecuteAsync(
        ReportInput input,        CancellationToken ct)
    {
        ReportProgress(25, "Preparing report");

        await Task.Delay(500, ct);

        ReportProgress(75, "Finalizing report");

        await Task.Delay(500, ct);

        ReportProgress(100, "Report generated");

        return await NextAsync<SendEmailAction>(
            new EmailInput
            {
                To = "admin@demo.local",
                Subject = $"Imported {input.ImportedCustomers} customers"
            });
    }
}

public sealed class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string From { get; set; } = string.Empty;
}

[ActionName(Actions.SendEmail)]
public sealed class SendEmailAction(SmtpSettings smtp)    : JobAction<EmailInput>
{
    public override Task<IActionResult> ExecuteAsync(
        EmailInput input, CancellationToken ct)
    {
        ReportProgress(100, $"Email sent via {smtp.Host}");

        return CompleteAsync();
    }
}