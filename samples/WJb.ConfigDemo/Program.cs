using WJb;
using WJb.ConfigDemo;
using WJb.Helpers;

// ====================================
// Configuration API
// ====================================

var store1 = new InMemoryStore();

var wjb1 = WJbBuilder.Create(store1, cfg =>
{
    cfg.AddAction<SendEmailAction>(
        Actions.SendEmail,
        more: new
        {
            smtpCode = SmtpSettings.Key
        });

    cfg.AddService(
        SmtpSettings.Key,
        new SmtpSettings
        {
            Host = "smtp.local"
        });
});

Console.WriteLine("CONFIGURATION API");
await RunAsync(wjb1);

// ====================================
// JSON Configuration
// ====================================

var store2 = new InMemoryStore();

var wjb2 = WJbBuilder.Create(store2, cfg =>
{
    cfg.AddActionsFromJson(CreateActionsJson());
    cfg.AddServicesFromJson(CreateServicesJson());
});

Console.WriteLine("JSON CONFIGURATION");
await RunAsync(wjb2);


// ====================================
// Store Configuration
// ====================================

var store3 = new InMemoryStore();

await store3.SetAsync(DefinitionType.Actions, Actions.SendEmail,
    new
    {
        type = TypeHelper.GetName(typeof(SendEmailAction)),
        smtpCode = SmtpSettings.Key
    });

await store3.SetAsync(DefinitionType.Services, SmtpSettings.Key,
    new
    {
        type = TypeHelper.GetName(typeof(SmtpSettings)),
        host = "smtp.store.local"
    });

var wjb3 = await WJbBuilder.CreateAsync(store3);
Console.WriteLine("STORE CONFIGURATION");

await RunAsync(wjb3);

// ====================================
// Helpers
// ====================================

static async Task RunAsync(IWJb wjb)
{
    await wjb.EnqueueAsync(
        Actions.SendEmail,
        new EmailInput
        {
            To = "test@test.com"
        });

    await wjb.ExecuteOnceAsync();

    Console.WriteLine();
}

static string CreateActionsJson()
{
    return $$"""
    {
      "{{Actions.SendEmail}}":
      {
        "type": "{{TypeHelper.GetName(typeof(SendEmailAction))}}",
        "smtpCode": "{{SmtpSettings.Key}}"
      }
    }
    """;
}

static string CreateServicesJson()
{
    return $$"""
    {
      "{{SmtpSettings.Key}}":
      {
        "type": "{{TypeHelper.GetName(typeof(SmtpSettings))}}",
        "host": "smtp.json.local"
      }
    }
    """;
}

// ====================================
// Demo Types
// ====================================

public static class Actions
{
    public const string SendEmail = "send-email";
}

namespace WJb.ConfigDemo
{
    public sealed class SendEmailAction(SmtpSettings smtp) : JobAction<EmailInput>
    {
        private readonly SmtpSettings _smtp = smtp;

        public override Task<ActionResult> ExecuteAsync(EmailInput input, CancellationToken ct = default)
        {
            Console.WriteLine($"SMTP: {_smtp?.Host}, To: {input?.To ?? "<no recipient>"}");

            return Task.FromResult(ActionResults.None());
        }
    }

    public sealed class EmailInput
    {
        public string? To { get; set; }
    }

    public sealed class SmtpSettings
    {
        public const string Key = "smtp";

        public string Host { get; set; } = default!;
    }
}