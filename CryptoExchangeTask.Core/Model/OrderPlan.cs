namespace CryptoExchangeTask.Core.Model;

public class OrderPlan
{
    public IReadOnlyList<PlannedOrder> PlannedOrders { get; init; } = new List<PlannedOrder>();
}
