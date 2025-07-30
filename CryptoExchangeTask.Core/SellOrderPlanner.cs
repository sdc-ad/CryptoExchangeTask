using CryptoExchangeTask.Core.Model;
using CryptoExchangeTask.Core.Repository;

namespace CryptoExchangeTask.Core;

/// <summary>
/// Given a user request to sell, determines the optimum set of orders which should be placed to
/// fulfill that request.
/// </summary>
public class SellOrderPlanner : OrderPlanner
{
    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="exchangeRepository">
    /// The repository from which to fetch the exchanges, their order books and the available balance.
    /// </param>
    public SellOrderPlanner(IExchangeRepository exchangeRepository) : base(exchangeRepository)
    {
    }

    // Generated orders are always "sell"
    protected override OrderType OrderType => OrderType.Sell;

    // When selling we will consume the Crypto balance at each exchange
    protected override decimal AvailableBalance(AvailableFunds availableFunds) => availableFunds.Crypto;

    // Balance is the Crypto amount, no conversion is necessary
    protected override decimal BalanceToAmount(decimal balance, decimal price) => balance;

    // Balance is the Crypto amount, no conversion is necessary
    protected override decimal AmountToBalance(decimal amount, decimal price) => amount;

    // Sells are fulfilled using Bid order book entries
    protected override IEnumerable<OrderBookEntry> OrderBookEntries(OrderBook orderBook) => orderBook.Bids;

    // When selling we want the highest price first
    protected override IEnumerable<OrderPlanState> SortOrderPlanStates(IEnumerable<OrderPlanState> entries) =>
        entries.OrderByDescending(o => o.Price);
}
