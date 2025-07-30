using CryptoExchangeTask.Core.Errors;
using CryptoExchangeTask.Core.Model;
using CryptoExchangeTask.Core.UnitTests.Mock;
using Shouldly;

namespace CryptoExchangeTask.Core.UnitTests.OrderPlannerTests;

/// <summary>
/// Tests creating "buy" plans.
/// </summary>
public class BuyTests : OrderPlannerTestBase
{
    /// <summary>
    /// Given a single exchange, with a single order, uses that order to fulfill the request.
    /// </summary>
    [Fact]
    public void PlansBuyFromSingleExchangeSingleOrder()
    {
        var askOrderBookEntry = MakeEntry(3, 250);
        var exchange = MakeExchange(10, 1000, [], [askOrderBookEntry]);

        var plan = MakePlanner(exchange).Plan(2);

        plan.ShouldBeEquivalentTo(
            MakeExpectedPlan(
                MakeExpectedOrder(exchange, askOrderBookEntry, 2)));

        plan.TotalAmount.ShouldBe(2m);
        plan.TotalPrice.ShouldBe(500m);
    }

    /// <summary>
    /// Given a single exchange with multiple orders, splits the request across the orders, using the 
    /// cheaper order first.
    /// </summary>
    [Fact]
    public void PlansBuyFromSingleExchangeMultipleOrders()
    {
        var askOrderBookEntry1 = MakeEntry(3, 250);
        var askOrderBookEntry2 = MakeEntry(3, 500);
        var exchange = MakeExchange(10, 10000, [], [askOrderBookEntry1, askOrderBookEntry2]);

        var plan = MakePlanner(exchange).Plan(4);

        plan.ShouldBeEquivalentTo(MakeExpectedPlan(
            MakeExpectedOrder(exchange, askOrderBookEntry1, 3),
            MakeExpectedOrder(exchange, askOrderBookEntry2, 1)));
    }

    /// <summary>
    /// Given multiple exchanges with multiple orders, splits the request across the orders,
    /// using the cheaper orders first, even if they are at different exchanges.
    /// </summary>
    [Fact]
    public void PlansBuyFromMultipleExchanges()
    {
        var exchange1Ask1 = MakeEntry(3, 250);
        var exchange1Ask2 = MakeEntry(3, 750);
        var exchange1 = MakeExchange(10, 10000, [], [exchange1Ask1, exchange1Ask2]);

        var exchange2Ask1 = MakeEntry(3, 500);
        var exchange2 = MakeExchange(10, 10000, [], [exchange2Ask1]);

        var plan = MakePlanner(exchange1, exchange2).Plan(4);

        plan.ShouldBeEquivalentTo(MakeExpectedPlan(
            MakeExpectedOrder(exchange1, exchange1Ask1, 3),
            MakeExpectedOrder(exchange2, exchange2Ask1, 1)));

        plan.TotalAmount.ShouldBe(4m);
        plan.TotalPrice.ShouldBe(1250m);
    }

    /// <summary>
    /// Given multiple exchanges with multiple orders, splits the request across the orders, using the cheapest orders first,
    /// but following the available balance at each exchange.
    /// </summary>
    [Fact]
    public void PlansBuyFromMultipleExchangesLimitedBalance()
    {
        var exchange1Ask1 = MakeEntry(1, 250);
        var exchange1Ask2 = MakeEntry(2, 250);
        var exchange1 = MakeExchange(10, 375, [], [exchange1Ask1, exchange1Ask2]);

        var exchange2Ask1 = MakeEntry(3, 500);
        var exchange2 = MakeExchange(10, 10000, [], [exchange2Ask1]);

        var plan = MakePlanner(exchange1, exchange2).Plan(4);

        plan.ShouldBeEquivalentTo(MakeExpectedPlan(
            MakeExpectedOrder(exchange1, exchange1Ask1, 1m),
            MakeExpectedOrder(exchange1, exchange1Ask2, 0.5m),
            MakeExpectedOrder(exchange2, exchange2Ask1, 2.5m)));
    }

    /// <summary>
    /// Given exchanges where the balance is insufficient to fulfill the request, throws an exception
    /// </summary>
    [Fact]
    public void InvalidPlansBuyBalanceUnavailable()
    {
        var exchange1Ask1 = MakeEntry(3, 250);
        var exchange1Ask2 = MakeEntry(3, 750);
        var exchange1 = MakeExchange(10, 375, [], [exchange1Ask1, exchange1Ask2]);

        var exchange2Ask1 = MakeEntry(3, 500);
        var exchange2 = MakeExchange(10, 600, [], [exchange2Ask1]);

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
        var exchange1Ask1 = MakeEntry(3, 250);
        var exchange1 = MakeExchange(10, 375, [], [exchange1Ask1]);

        Assert.Throws<ArgumentException>(() => MakePlanner(exchange1).Plan(amount));
    }

    /// <summary>
    /// Create an instance of the "buy" planner.
    /// </summary>
    /// <param name="exchanges">The test exchange data.</param>
    /// <returns>An instance of the planner.</returns>
    private static OrderPlanner MakePlanner(params Exchange[] exchanges)
    {
        return new BuyOrderPlanner(new TestExchangeRepository
        {
            Exchanges = exchanges.ToList()
        });
    }

    /// <summary>
    /// Make an expected "buy" Planned Order object.
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
            OrderType: OrderType.Buy,
            Amount: amount,
            Price: fulfilledByOrder.Order.Price);
    }
}
