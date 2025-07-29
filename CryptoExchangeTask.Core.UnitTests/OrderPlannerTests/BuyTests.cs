using CryptoExchangeTask.Core.Errors;
using CryptoExchangeTask.Core.Model;
using CryptoExchangeTask.Core.UnitTests.Mock;
using Shouldly;

namespace CryptoExchangeTask.Core.UnitTests.OrderPlannerTests;
public class BuyTests : OrderPlannerTestBase
{
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

    private static OrderPlanner MakePlanner(params Exchange[] exchanges)
    {
        return new BuyOrderPlanner(new TestExchangeRepository
        {
            Exchanges = exchanges.ToList()
        });
    }
}
