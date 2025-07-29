using CryptoExchangeTask.Core.Errors;
using CryptoExchangeTask.Core.Model;
using CryptoExchangeTask.Core.UnitTests.Mock;
using Shouldly;

namespace CryptoExchangeTask.Core.UnitTests.OrderPlannerTests;

public class SellTests : OrderPlannerTestBase
{
    [Fact]
    public void PlansSellFromSingleExchangeSingleOrder()
    {
        var bidOrderBookEntry = MakeEntry(3, 250);
        var exchange = MakeExchange(10, 1000, [bidOrderBookEntry], []);

        var plan = MakePlanner(exchange).Plan(2);

        plan.ShouldBeEquivalentTo(
            MakeExpectedPlan(
                MakeExpectedOrder(exchange, bidOrderBookEntry, 2)));
    }

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
    }

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

    private static OrderPlanner MakePlanner(params Exchange[] exchanges)
    {
        return new SellOrderPlanner(new TestExchangeRepository
        {
            Exchanges = exchanges.ToList()
        });
    }
}
