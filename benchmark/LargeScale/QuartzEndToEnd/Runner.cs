using Quartz;

public static class Runner
{
    public const int JobCount = 1_000_000;

    public static async Task ExecuteAsync()
    {
        QuartzEchoJob.Completed = 0;
        QuartzEchoJob.Expected = JobCount;

        QuartzEchoJob.Completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        var scheduler = await QuartzSchedulerBuilder
            .Create(q =>
            {
                q.UseInMemoryStore();

                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 1;
                });
            })
            .BuildScheduler();

        await scheduler.Start();

        for (int i = 0; i < JobCount; i++)
        {
            var payload =
                new EchoPayload(i, $"User {i}");

            var job = JobBuilder.Create<QuartzEchoJob>()
                .WithIdentity($"job-{i}")
                .UsingJobData(new JobDataMap
                {
                    ["payload"] = payload
                })
                .Build();

            await scheduler.ScheduleJob(
                job,
                TriggerBuilder.Create()
                    .StartNow()
                    .Build());
        }

        await QuartzEchoJob.Completion.Task;

        if (QuartzEchoJob.Completed != JobCount)
        {
            throw new InvalidOperationException(
                $"Expected={JobCount}, Actual={QuartzEchoJob.Completed}");
        }

        Console.WriteLine(
            $"Completed   : {QuartzEchoJob.Completed:N0}");

        await scheduler.Shutdown(true);
    }
}