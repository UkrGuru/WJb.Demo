namespace WJb.IntegrationTests;

/// <summary>
/// Available only in the commercial edition.
/// Demonstrates recurring CRON jobs.
/// Equivalent to Hangfire RecurringJob.AddOrUpdate().
/// </summary>
public class _03_RecurringJobTests
{
    [Fact]
    public async Task Should_Create_Jobs_From_Cron_Schedule()
    {
    }
}

// Verifies:
//
// - Scheduler creates jobs from a CRON schedule.
// - Multiple scheduler runs work correctly.
// - Available only in the commercial edition.