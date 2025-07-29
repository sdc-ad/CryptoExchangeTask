using CryptoExchangeTask.Core.Model;
using CryptoExchangeTask.Core.UnitTests.Mock;

namespace CryptoExchangeTask.Core.UnitTests.OrderPlannerTests;

public abstract class OrderPlannerTestBase
{
    protected static OrderBookEntry MakeEntry(decimal amount, decimal price)
    {
        return new OrderBookEntry
        {
            Order = new()
            {
                Amount = amount,
                Price = price
            }
        };
    }

    protected static Exchange MakeExchange(decimal availableCrypto, decimal availableEuro, List<OrderBookEntry> bids, List<OrderBookEntry> asks)
    {
        return new Exchange
        {
            Id = Guid.NewGuid().ToString(),
            AvailableFunds = new()
            {
                Crypto = availableCrypto,
                Euro = availableEuro
            },
            OrderBook = new()
            {
                Bids = bids,
                Asks = asks
            }
        };
    }

    protected static PlannedOrder MakeExpectedOrder(Exchange fulfilledByExchange, OrderBookEntry fulfilledByOrder, decimal amount)
    {
        return new PlannedOrder
        {
            FulfilledByExchangeId = fulfilledByExchange.Id,
            FulfilledByOrderId = fulfilledByOrder.Order.Id,
            Amount = amount,
            Price = fulfilledByOrder.Order.Price
        };
    }

    protected static OrderPlan MakeExpectedPlan(params PlannedOrder[] plannedOrders)
    {
        return new OrderPlan
        {
            PlannedOrders = plannedOrders.ToList()
        };
    }
}
