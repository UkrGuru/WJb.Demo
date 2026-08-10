await WJb.EnqueueAsync(
    RetryEmailAction.Key,
    payload: new
    {
        payload = new RetryEmailInput
        {
            To = "test@test.com"
        },
        options = new JobOptions
        {
            MaxRetries = 1,
            RetryDelay = TimeSpan.FromSeconds(1)
        }
    });