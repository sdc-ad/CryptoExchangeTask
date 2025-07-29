using CryptoExchangeTask.Core.Model;
using CryptoExchangeTask.Core.UnitTests.Mock;
using Shouldly;

namespace CryptoExchangeTask.Core.UnitTests;

public class OrderPlannerTests
{
    [Fact]
    public void PlansBuyFromSingleExchangeSingleOrder()
    {
        var askOrderBookEntry = MakeEntry(3, 250);
        var exchange = MakeExchange(2000, 1000, [], [askOrderBookEntry]);

        var plan = MakePlanner(exchange).Plan(OrderType.Buy, 2);

        plan.ShouldBeEquivalentTo(new OrderPlan
        {
            PlannedOrders =
            {
                new()
                {
                    FulfilledByExchangeId = exchange.Id,
                    FulfilledByOrderId = askOrderBookEntry.Order.Id,
                    Amount = 2,
                    Price = 250
                }
            }
        });
    }

    private static OrderBookEntry MakeEntry(decimal amount, decimal price)
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

    private static Exchange MakeExchange(decimal availableCrypto, decimal availableEuro, List<OrderBookEntry> bids, List<OrderBookEntry> asks)
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

    private static OrderPlanner MakePlanner(params Exchange[] exchanges)
    {
        return new OrderPlanner(new TestExchangeRepository
        {
            Exchanges = exchanges.ToList()
        });
    }
}
