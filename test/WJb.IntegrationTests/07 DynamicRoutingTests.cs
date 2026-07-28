namespace WJb.IntegrationTests;

/// <summary>
/// Demonstrates dynamic workflow routing.
/// </summary>
public class _07_DynamicRoutingTests
{
    [Fact]
    public async Task Should_Route_To_Ship_When_Approved()
    {
        await ExecuteAsync(approved: true, expected: "Ship");
    }

    [Fact]
    public async Task Should_Route_To_Cancel_When_Rejected()
    {
        await ExecuteAsync(approved: false, expected: "Cancel");
    }

    private static async Task ExecuteAsync(
        bool approved,
        string expected)
    {
        RoutingState.Executed.Clear();

        var store = new InMemoryStore();

        var runtime = WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<OrderAction>("order");
            cfg.AddAction<ShipAction>("ship");
            cfg.AddAction<CancelAction>("cancel");
        });

        await runtime.EnqueueAsync("order",
            new OrderInput
            {
                Approved = approved
            });

        await runtime.ExecuteLoopAsync();

        Assert.Contains("Order", RoutingState.Executed);
        Assert.Contains(expected, RoutingState.Executed);

        Assert.Equal(2, RoutingState.Executed.Count);

        var completed = await store.GetJobsAsync(
            new JobQueryInfo
            {
                Status = JobStatus.Completed
            });

        Assert.Equal(2, completed.Count);
    }

    public sealed class OrderInput
    {
        public bool Approved { get; set; }
    }

    public sealed class OrderAction : JobAction<OrderInput>
    {
        public override Task<ActionResult> ExecuteAsync(OrderInput input, CancellationToken ct = default)
        {
            RoutingState.Executed.Add("Order");

            var next = input.Approved ? "ship" : "cancel";

            return Task.FromResult(ActionResults.Next(new JobCommand(next)));
        }
    }

    public sealed class ShipAction : JobAction<object>
    {
        public override Task<ActionResult> ExecuteAsync(object input, CancellationToken ct = default)
        {
            RoutingState.Executed.Add("Ship");

            return Task.FromResult(ActionResults.None());
        }
    }

    public sealed class CancelAction : JobAction<object>
    {
        public override Task<ActionResult> ExecuteAsync(object input, CancellationToken ct = default)
        {
            RoutingState.Executed.Add("Cancel");

            return Task.FromResult(ActionResults.None());
        }
    }

    private static class RoutingState
    {
        public static List<string> Executed { get; } = [];
    }
}