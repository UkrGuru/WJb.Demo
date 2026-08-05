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
    }

    [Fact]
    public void ActionResults_Result_Should_Support_Integer_Result()
    {
        var result = ActionResults.Result(123);

        Assert.Equal(123, result.Value);
    }

    [Fact]
    public void ActionResults_Result_Should_Support_String_Result()
    {
        var result = ActionResults.Result("Done");

        Assert.Equal("Done", result.Value);
    }

    [Fact]
    public void ActionResults_Result_Should_Support_Object_Result()
    {
        var result = ActionResults.Result(
            new
            {
                Sent = true
            });

        Assert.NotNull(result.Value);
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
    }
}