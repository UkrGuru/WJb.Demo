namespace WJb.Demo.Monitor;

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

    public override async Task<ActionResult> ExecuteAsync(
        ImportCustomersInput input,
        CancellationToken ct)
    {
        for (var i = 0; i <= 100; i += 25)
        {
            ReportProgress(i, $"Importing customers {i}%");

            await Task.Delay(500, ct);
        }

        return ActionResults.Next(
            new JobCommand(
                GenerateReportAction.Key,
                new ReportInput
                {
                    ImportedCustomers = 1250
                }));
    }
}

public sealed class EmailInput
{
    public string To { get; set; } = "";
    public string Subject { get; set; } = "";
}

public sealed class GenerateReportAction
    : JobAction<ReportInput>, IProgressAction
{
    public const string Key = "generate-report";

    public override async Task<ActionResult> ExecuteAsync(
        ReportInput input,
        CancellationToken ct)
    {
        ReportProgress(25, "Preparing report");

        await Task.Delay(500, ct);

        ReportProgress(75, "Finalizing report");

        await Task.Delay(500, ct);

        ReportProgress(100, "Report generated");

        return ActionResults.Next(
            new JobCommand(
                SendEmailAction.Key,
                new EmailInput
                {
                    To = "admin@demo.local",
                    Subject = $"Imported {input.ImportedCustomers} customers"
                }));
    }
}

public sealed class SmtpSettings
{
    public const string Key = "smtp";

    public string Host { get; set; } = "";
    public int Port { get; set; }
    public string From { get; set; } = "";
}

public sealed class SendEmailAction(SmtpSettings smtp)
    : JobAction<EmailInput>
{
    public const string Key = "send-email";

    public override Task<ActionResult> ExecuteAsync(
        EmailInput input,
        CancellationToken ct)
    {
        Console.WriteLine(
            $"SMTP: {smtp.Host}:{smtp.Port}");

        Console.WriteLine(
            $"From: {smtp.From}");

        Console.WriteLine(
            $"To: {input.To}");

        Console.WriteLine(
            $"Subject: {input.Subject}");

        return Task.FromResult(
            ActionResults.None());
    }
}