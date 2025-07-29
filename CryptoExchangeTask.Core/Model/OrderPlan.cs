namespace CryptoExchangeTask.Core.Model;

public class OrderPlan
{
    public IReadOnlyList<PlannedOrder> PlannedOrders { get; init; } = new List<PlannedOrder>();

    public decimal TotalPrice => PlannedOrders.Sum(o => o.Price * o.Amount);

    public decimal TotalAmount => PlannedOrders.Sum(o => o.Amount);
}
