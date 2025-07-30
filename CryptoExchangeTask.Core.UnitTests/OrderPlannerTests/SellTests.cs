using CryptoExchangeTask.Core.Errors;
using CryptoExchangeTask.Core.Model;
using CryptoExchangeTask.Core.UnitTests.Mock;
using Shouldly;

namespace CryptoExchangeTask.Core.UnitTests.OrderPlannerTests;

/// <summary>
/// Tests creating "buy" plans.
/// </summary>
public class SellTests : OrderPlannerTestBase
{
    /// <summary>
    /// Given a single exchange, with a single order, uses that order to fulfill the request.
    /// </summary>
    [Fact]
    public void PlansSellFromSingleExchangeSingleOrder()
    {
        var bidOrderBookEntry = MakeEntry(3, 250);
        var exchange = MakeExchange(10, 1000, [bidOrderBookEntry], []);

        var plan = MakePlanner(exchange).Plan(2);

        plan.ShouldBeEquivalentTo(
            MakeExpectedPlan(
                MakeExpectedOrder(exchange, bidOrderBookEntry, 2)));

        plan.TotalAmount.ShouldBe(2m);
        plan.TotalPrice.ShouldBe(500m);
    }

    /// <summary>
    /// Given a single exchange with multiple orders, splits the request across the orders, using the 
    /// higher priced order first.
    /// </summary>
    [Fact]
    public void PlansSellFromSingleExchangeMultipleOrders()
    {
        var bidOrderBookEntry1 = MakeEntry(3, 250);
        var bidOrderBookEntry2 = MakeEntry(3, 500);
        var exchange = MakeExchange(10, 10000, [bidOrderBookEntry1, bidOrderBookEntry2], []);

        var plan = MakePlanner(exchange).Plan(4);

        plan.ShouldBeEquivalentTo(MakeExpectedPlan(
            MakeExpectedOrder(exchange, bidOrderBookEntry2, 3),
            MakeExpectedOrder(exchange, bidOrderBookEntry1, 1)));
    }

    /// <summary>
    /// Given multiple exchanges with multiple orders, splits the request across the orders,
    /// using the higher priced orders first, even if they are at different exchanges.
    /// </summary>
    [Fact]
    public void PlansSellFromMultipleExchanges()
    {
        var exchange1Bid1 = MakeEntry(3, 250);
        var exchange1Bid2 = MakeEntry(3, 750);
        var exchange1 = MakeExchange(10, 10000, [exchange1Bid1, exchange1Bid2], []);

        var exchange2Bid1 = MakeEntry(3, 500);
        var exchange2 = MakeExchange(10, 10000, [exchange2Bid1], []);

        var plan = MakePlanner(exchange1, exchange2).Plan(4);

        plan.ShouldBeEquivalentTo(MakeExpectedPlan(
            MakeExpectedOrder(exchange1, exchange1Bid2, 3),
            MakeExpectedOrder(exchange2, exchange2Bid1, 1)));

        plan.TotalAmount.ShouldBe(4m);
        plan.TotalPrice.ShouldBe(2750m);
    }

    /// <summary>
    /// Given multiple exchanges with multiple orders, splits the request across the orders, using the higher priced orders first,
    /// but following the available balance at each exchange.
    /// </summary>
    [Fact]
    public void PlansSellFromMultipleExchangesLimitedBalance()
    {
        var exchange1Bid1 = MakeEntry(1, 750);
        var exchange1Bid2 = MakeEntry(2, 150);
        var exchange1 = MakeExchange(10, 375, [exchange1Bid1, exchange1Bid2], []);

        var exchange2Bid1 = MakeEntry(3, 500);
        var exchange2 = MakeExchange(2, 10000, [exchange2Bid1], []);

        var plan = MakePlanner(exchange1, exchange2).Plan(4);

        plan.ShouldBeEquivalentTo(MakeExpectedPlan(
            MakeExpectedOrder(exchange1, exchange1Bid1, 1m),
            MakeExpectedOrder(exchange2, exchange2Bid1, 2m),
            MakeExpectedOrder(exchange1, exchange1Bid2, 1m)));
    }

    /// <summary>
    /// Given exchanges where the balance is insufficient to fulfill the request, throws an exception
    /// </summary>
    [Fact]
    public void InvalidPlansSellBalanceUnavailable()
    {
        var exchange1Bid1 = MakeEntry(3, 250);
        var exchange1Bid2 = MakeEntry(3, 750);
        var exchange1 = MakeExchange(2, 375, [exchange1Bid1, exchange1Bid2], []);

        var exchange2Bid1 = MakeEntry(3, 500);
        var exchange2 = MakeExchange(1, 600, [exchange2Bid1], []);

        Assert.Throws<InsufficientBalanceException>(() => MakePlanner(exchange1, exchange2).Plan(4));
    }

    /// <summary>
    /// Throws an exception if the requested amount is <= 0
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void InvalidPlansBuyAmountLessThan0(int amount)
    {
        var exchange1Bid1 = MakeEntry(3, 250);
        var exchange1 = MakeExchange(10, 375, [exchange1Bid1], []);

        Assert.Throws<ArgumentException>(() => MakePlanner(exchange1).Plan(amount));
    }

    /// <summary>
    /// Create an instance of the "sell" planner.
    /// </summary>
    /// <param name="exchanges">The test exchange data.</param>
    /// <returns>An instance of the planner.</returns>
    private static OrderPlanner MakePlanner(params Exchange[] exchanges)
    {
        return new SellOrderPlanner(new TestExchangeRepository
        {
            Exchanges = exchanges.ToList()
        });
    }

    /// <summary>
    /// Make an expected "sell" Planned Order object.
    /// </summary>
    /// <param name="fulfilledByExchange">The exchange which is expected to be used to fulfill this order.</param>
    /// <param name="fulfilledByOrder">The order book entry which is expected to be used to fulfill this order.</param>
    /// <param name="amount">The expected amount of the planned order.</param>
    /// <returns>A Planned Order object.</returns>
    private static PlannedOrder MakeExpectedOrder(Exchange fulfilledByExchange, OrderBookEntry fulfilledByOrder, decimal amount)
    {
        return new PlannedOrder(
            FulfilledByExchangeId: fulfilledByExchange.Id,
            FulfilledByOrderId: fulfilledByOrder.Order.Id,
            OrderType: OrderType.Sell,
            Amount: amount,
            Price: fulfilledByOrder.Order.Price);
    }
}
