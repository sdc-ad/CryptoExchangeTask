namespace CryptoExchangeTask.Core.Model;

public record OrderPlan(
    IReadOnlyList<PlannedOrder> PlannedOrders)
{
    public decimal TotalPrice => PlannedOrders.Sum(o => o.Price * o.Amount);

    public decimal TotalAmount => PlannedOrders.Sum(o => o.Amount);
}
