using WJb;
using Xunit;

namespace WJb.IntegrationTests;

/// <summary>
/// Demonstrates constructor dependency injection.
/// </summary>
public class _11_DependencyInjectionTests
{
    [Fact]
    public async Task Should_Inject_Service()
    {
        var runtime = WJbBuilder.Create(cfg =>
        {
            cfg.AddAction<SendEmailAction>("email");

            cfg.AddService(new SmtpSettings
            {
                Host = "smtp.local"
            });
        });

        var action =
            (SendEmailAction)await runtime.CreateAsync("email");

        Assert.NotNull(action.Settings);
        Assert.Equal("smtp.local", action.Settings!.Host);
    }

    public sealed class SendEmailAction(
        SmtpSettings? settings)
        : JobAction<EmailInput>
    {
        public SmtpSettings? Settings { get; } = settings;

        public override Task<ActionResult> ExecuteAsync(
            EmailInput input,
            CancellationToken ct = default)
        {
            return Task.FromResult(ActionResults.None());
        }
    }

    public sealed class EmailInput
    {
        public string? To { get; set; }
    }

    public sealed class SmtpSettings
    {
        public string Host { get; set; } = default!;
    }
}