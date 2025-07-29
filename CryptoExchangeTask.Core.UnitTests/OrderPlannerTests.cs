using CryptoExchangeTask.Core.Errors;
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

        plan.ShouldBeEquivalentTo(
            MakeExpectedPlan(
                MakeExpectedOrder(exchange, askOrderBookEntry, 2)));
    }

    [Fact]
    public void PlansBuyFromSingleExchangeMultipleOrders()
    {
        var askOrderBookEntry1 = MakeEntry(3, 250);
        var askOrderBookEntry2 = MakeEntry(3, 500);
        var exchange = MakeExchange(2000, 10000, [], [askOrderBookEntry1, askOrderBookEntry2]);

        var plan = MakePlanner(exchange).Plan(OrderType.Buy, 4);

        plan.ShouldBeEquivalentTo(MakeExpectedPlan(
            MakeExpectedOrder(exchange, askOrderBookEntry1, 3),
            MakeExpectedOrder(exchange, askOrderBookEntry2, 1)));
    }

    [Fact]
    public void PlansBuyFromMultipleExchanges()
    {
        var exchange1Ask1 = MakeEntry(3, 250);
        var exchange1Ask2 = MakeEntry(3, 750);
        var exchange1 = MakeExchange(2000, 10000, [], [exchange1Ask1, exchange1Ask2]);

        var exchange2Ask1 = MakeEntry(3, 500);
        var exchange2 = MakeExchange(2000, 10000, [], [exchange2Ask1]);

        var plan = MakePlanner(exchange1, exchange2).Plan(OrderType.Buy, 4);

        plan.ShouldBeEquivalentTo(MakeExpectedPlan(
            MakeExpectedOrder(exchange1, exchange1Ask1, 3),
            MakeExpectedOrder(exchange2, exchange2Ask1, 1)));
    }

    [Fact]
    public void PlansBuyFromMultipleExchangesLimitedBalance()
    {
        var exchange1Ask1 = MakeEntry(3, 250);
        var exchange1Ask2 = MakeEntry(3, 750);
        var exchange1 = MakeExchange(2000, 375, [], [exchange1Ask1, exchange1Ask2]);

        var exchange2Ask1 = MakeEntry(3, 500);
        var exchange2 = MakeExchange(2000, 10000, [], [exchange2Ask1]);

        var plan = MakePlanner(exchange1, exchange2).Plan(OrderType.Buy, 4);

        plan.ShouldBeEquivalentTo(MakeExpectedPlan(
            MakeExpectedOrder(exchange1, exchange1Ask1, 1.5m),
            MakeExpectedOrder(exchange2, exchange2Ask1, 2.5m)));
    }

    [Fact]
    public void InvalidPlansBuyBalanceUnavailable()
    {
        var exchange1Ask1 = MakeEntry(3, 250);
        var exchange1Ask2 = MakeEntry(3, 750);
        var exchange1 = MakeExchange(2000, 375, [], [exchange1Ask1, exchange1Ask2]);

        var exchange2Ask1 = MakeEntry(3, 500);
        var exchange2 = MakeExchange(2000, 600, [], [exchange2Ask1]);

        Assert.Throws<InsufficientBalanceException>(() => MakePlanner(exchange1, exchange2).Plan(OrderType.Buy, 4));
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

    private static PlannedOrder MakeExpectedOrder(Exchange fulfilledByExchange, OrderBookEntry fulfilledByOrder, decimal amount)
    {
        return new PlannedOrder
        {
            FulfilledByExchangeId = fulfilledByExchange.Id,
            FulfilledByOrderId = fulfilledByOrder.Order.Id,
            Amount = amount,
            Price = fulfilledByOrder.Order.Price
        };
    }

    private static OrderPlan MakeExpectedPlan(params PlannedOrder[] plannedOrders)
    {
        return new OrderPlan
        {
            PlannedOrders = plannedOrders.ToList()
        };
    }
}
