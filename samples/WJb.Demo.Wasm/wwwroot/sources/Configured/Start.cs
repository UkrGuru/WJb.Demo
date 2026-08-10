await Store.EnqueueAsync(
    ConfiguredAction.Key,
    new EmailInput
    {
        To = "test@test.com"
    });