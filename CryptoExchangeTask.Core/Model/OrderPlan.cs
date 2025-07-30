namespace CryptoExchangeTask.Core.Model;

/// <summary>
/// A set of planned orders which would fulfill a user's request.
/// </summary>
/// <param name="PlannedOrders">The planned orders to be executed.</param>
public record OrderPlan(
    IReadOnlyList<PlannedOrder> PlannedOrders)
{
    /// <summary>The total Euro price of the planned orders</summary>
    public decimal TotalPrice => PlannedOrders.Sum(o => o.Price * o.Amount);

    /// <summary>The total crypto amount of the planned orders.</summary>
    public decimal TotalAmount => PlannedOrders.Sum(o => o.Amount);
}
