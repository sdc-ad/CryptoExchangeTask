using CryptoExchangeTask.Core.Model;
using CryptoExchangeTask.Core.Repository;

namespace CryptoExchangeTask.Core;

public class SellOrderPlanner : OrderPlanner
{
    public SellOrderPlanner(IExchangeRepository exchangeRepository) : base(exchangeRepository)
    {
    }

    protected override decimal AmountToBalance(decimal amount, decimal price) => amount;

    protected override decimal BalanceToAmount(decimal balance, decimal price) => balance;

    protected override decimal AvailableBalance(AvailableFunds availableFunds) => availableFunds.Crypto;

    protected override IEnumerable<OrderBookEntry> OrderBookEntries(OrderBook orderBook) => orderBook.Bids;

    protected override IEnumerable<OrderPlanState> SortOrderPlanStates(IEnumerable<OrderPlanState> entries) =>
        entries.OrderByDescending(o => o.Price);
}
