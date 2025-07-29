namespace CryptoExchangeTask.Core.Model;

public class OrderPlan
{
    public List<PlannedOrder> PlannedOrders { get; init; } = new();
}
