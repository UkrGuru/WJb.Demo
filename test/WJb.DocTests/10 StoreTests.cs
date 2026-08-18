namespace WJb.DocTests;

public class _10_StoreTests
{
    [Fact]
    public void InMemoryStore_Should_Be_Creatable()
    {
        var store = new InMemoryStore();

        Assert.NotNull(store);
    }

    [Fact]
    public void JobCommand_Should_Support_Guid_Payload_Values()
    {
        var id = Guid.NewGuid();

        var command = new JobCommand(
            "test",
            new
            {
                Id = id
            });

        var payload = command.AsObject();

        Assert.NotNull(payload);
    }

    [Fact]
    public void JobCommand_Should_Support_Integer_Payload_Values()
    {
        var command = new JobCommand(
            "test",
            new
            {
                CustomerId = 42
            });

        var payload = command.AsObject();

        Assert.NotNull(payload);

        Assert.Equal(
            42,
            payload!["CustomerId"]!.GetValue<int>());
    }

    [Fact]
    public void JobCommand_Should_Support_String_Payload_Values()
    {
        var command = new JobCommand(
            "test",
            new
            {
                File = "report.pdf"
            });

        var payload = command.AsObject();

        Assert.NotNull(payload);

        Assert.Equal(
            "report.pdf",
            payload!["File"]!.GetValue<string>());
    }

    [Fact]
    public void Complete_Should_Support_Integer_Result()
    {
        var result = Results.Complete(123);

        var complete =
            Assert.IsType<CompleteResult>(result);

        Assert.Equal(
            123,
            complete.Value);
    }

    [Fact]
    public void Complete_Should_Support_String_Result()
    {
        var result = Results.Complete("Done");

        var complete =
            Assert.IsType<CompleteResult>(result);

        Assert.Equal(
            "Done",
            complete.Value);
    }

    [Fact]
    public void Complete_Should_Support_Object_Result()
    {
        var result = Results.Complete(
            new
            {
                Sent = true
            });

        var complete =
            Assert.IsType<CompleteResult>(result);

        Assert.NotNull(
            complete.Value);
    }

    [Fact]
    public void JobCommand_Should_Support_Small_Metadata_Payloads()
    {
        var command = new JobCommand(
            "send-email",
            new
            {
                BodyId = "html-123"
            });

        var payload = command.AsObject();

        Assert.NotNull(payload);

        Assert.Equal(
            "html-123",
            payload!["BodyId"]!.GetValue<string>());
    }
}