
using Quartz;

public static class Runner
{
    public const int JobCount = 1_000_000;

    public static async Task ExecuteAsync()
    {
        // Reset benchmark state BEFORE starting Quartz.
        QuartzEchoJob.Completed = 0;
        QuartzEchoJob.Expected = JobCount;
        QuartzEchoJob.Sum = 0;

        QuartzEchoJob.Completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        // Quartz 4.x: one worker to match the WJb benchmark.
        await using var factory = QuartzSchedulerBuilder
            .Create(q => q
                .UseDefaultThreadPool(maxConcurrency: 1)
                .UseInMemoryStore())
            .Build();

        var scheduler = await factory.GetScheduler();

        try
        {
            await scheduler.Start();

            for (int i = 0; i < JobCount; i++)
            {
                var job = JobBuilder
                    .Create<QuartzEchoJob>()
                    .UsingJobData(new JobDataMap
                    {
                        ["payload"] = new EchoPayload(
                            i,
                            $"User {i}")
                    })
                    .Build();

                var trigger = TriggerBuilder
                    .Create()
                    .StartNow()
                    .Build();

                await scheduler.ScheduleJob(job, trigger);
            }

            // Wait until all jobs have executed.
            await QuartzEchoJob.Completion.Task;
        }
        finally
        {
            // Wait for any currently executing jobs.
            await scheduler.Shutdown(
                waitForJobsToComplete: true);
        }

        // Verification.
        long expectedSum =
            (long)JobCount * (JobCount - 1) / 2;

        int completed =
            Volatile.Read(ref QuartzEchoJob.Completed);

        long sum =
            Interlocked.Read(ref QuartzEchoJob.Sum);

        Console.WriteLine($"Completed   : {completed:N0}");
        Console.WriteLine($"Expected    : {JobCount:N0}");
        Console.WriteLine($"Sum         : {sum:N0}");
        Console.WriteLine($"ExpectedSum : {expectedSum:N0}");

        if (completed != JobCount)
        {
            throw new InvalidOperationException(
                $"Completed={completed:N0}, Expected={JobCount:N0}");
        }

        if (sum != expectedSum)
        {
            throw new InvalidOperationException(
                $"Sum={sum:N0}, ExpectedSum={expectedSum:N0}");
        }

        Console.WriteLine("Verification: PASSED");
    }
}
