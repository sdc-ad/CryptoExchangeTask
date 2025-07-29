using CryptoExchangeTask.Core.Model;
using CryptoExchangeTask.Core.Repository;

namespace CryptoExchangeTask.Core;

public class BuyOrderPlanner : OrderPlanner
{
    public BuyOrderPlanner(IExchangeRepository exchangeRepository) : base(exchangeRepository)
    {
    }

    protected override decimal AmountToBalance(decimal amount, decimal price) => amount * price;

    protected override decimal BalanceToAmount(decimal balance, decimal price) => balance / price;

    protected override decimal AvailableBalance(AvailableFunds availableFunds) => availableFunds.Euro;

    protected override IEnumerable<OrderBookEntry> OrderBookEntries(OrderBook orderBook) => orderBook.Asks;

    protected override IEnumerable<OrderPlanState> SortOrderPlanStates(IEnumerable<OrderPlanState> entries) =>
        entries.OrderBy(o => o.Price);
}
