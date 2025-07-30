using CryptoExchangeTask.Core.Model;

namespace CryptoExchangeTask.Core.UnitTests.OrderPlannerTests;

/// <summary>
/// Base class for order planner tests providing some helper methods for constructing test data.
/// </summary>
public abstract class OrderPlannerTestBase
{
    /// <summary>
    /// Make an exchange order book entry.
    /// </summary>
    /// <param name="amount">The crypto amount of the order.</param>
    /// <param name="price">The Euro unit price of the order.</param>
    /// <returns>An Order Book Entry.</returns>
    protected static OrderBookEntry MakeEntry(decimal amount, decimal price)
    {
        return new OrderBookEntry(
            new(
                Id: Guid.NewGuid(),
                Amount: amount,
                Price: price));
    }

    /// <summary>
    /// Make an exchange.
    /// </summary>
    /// <param name="availableCrypto">The available crypto balance.</param>
    /// <param name="availableEuro">The available Euro balance.</param>
    /// <param name="bids">The Order Book bids.</param>
    /// <param name="asks">The Order Book asks.</param>
    /// <returns>An Exchange object.</returns>
    protected static Exchange MakeExchange(decimal availableCrypto, decimal availableEuro, List<OrderBookEntry> bids, List<OrderBookEntry> asks)
    {
        return new Exchange(
            Id: Guid.NewGuid().ToString(),
            AvailableFunds: new(
                Crypto: availableCrypto,
                Euro: availableEuro),
            OrderBook: new(
                Bids: bids,
                Asks: asks));
    }

    /// <summary>
    /// Makes an expected Order Plan object.
    /// </summary>
    /// <param name="plannedOrders">The expected planned orders.</param>
    /// <returns>An Order Plan object.</returns>
    protected static OrderPlan MakeExpectedPlan(params PlannedOrder[] plannedOrders)
    {
        return new OrderPlan(plannedOrders.ToList());
    }
}
