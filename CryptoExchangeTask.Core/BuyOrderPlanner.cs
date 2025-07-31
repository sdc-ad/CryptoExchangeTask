using CryptoExchangeTask.Core.Model;
using CryptoExchangeTask.Core.Repository;

namespace CryptoExchangeTask.Core;

/// <summary>
/// Given a user request to buy, determines the optimum set of orders which should be placed to
/// fulfill that request.
/// </summary>
public class BuyOrderPlanner : OrderPlanner
{
    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="exchangeRepository">
    /// The repository from which to fetch the exchanges, their order books and the available balance.
    /// </param>
    public BuyOrderPlanner(IExchangeRepository exchangeRepository) : base(exchangeRepository)
    {
    }

    // Generated orders are always "buy"
    protected override OrderType OrderType => OrderType.Buy;

    // When buying we will consume the Euro balance at each exchange
    protected override decimal AvailableBalance(AvailableFunds availableFunds) => availableFunds.Euro;

    // Balance is Euros so must be converted to an amount of crypto product
    //
    // I don't currently do any rounding here, so this can lead to long decimal amounts of crypto product
    // being planned. Presumably there is a limit to how precise this value is allowed to be and should
    // round to that.
    protected override decimal BalanceToAmount(decimal balance, decimal price) => balance / price;

    // Balance is Euros so the amount must be converted to a balance
    protected override decimal AmountToBalance(decimal amount, decimal price) => amount * price;

    // Buys are fulfilled using Ask order book entries
    protected override IEnumerable<OrderBookEntry> OrderBookEntries(OrderBook orderBook) => orderBook.Asks;

    // When buying we want the lowest price first
    protected override IEnumerable<OrderPlanState> SortOrderPlanStates(IEnumerable<OrderPlanState> entries) =>
        entries.OrderBy(o => o.Price);
}
