using CryptoExchangeTask.Core.Errors;
using CryptoExchangeTask.Core.Model;
using CryptoExchangeTask.Core.Repository;

namespace CryptoExchangeTask.Core;

/// <summary>
/// Given a user request to buy or sell, determines the optimum set of orders which should be placed to
/// fulfill that request.
/// </summary>
/// <remarks>
/// <para>
/// There are separate concrete implementations for buy or sell.
/// </para>
/// <para>
/// It is up to the caller to use the correct concrete implementation for the required order type.
/// </para>
/// </remarks>
public abstract class OrderPlanner
{
    /// <summary>The repository from which to fetch the exchanges, their order books and the available balance.</summary>
    private readonly IExchangeRepository _exchangeRepository;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="exchangeRepository">
    /// The repository from which to fetch the exchanges, their order books and the available balance.
    /// </param>
    protected OrderPlanner(IExchangeRepository exchangeRepository)
    {
        _exchangeRepository = exchangeRepository;
    }

    /// <summary>
    /// Create a plan to fulfill the requested order.
    /// </summary>
    /// <param name="amount">The amount of crypto product to buy or sell.</param>
    /// <returns>A plan containing the orders to be executed at each exchange.</returns>
    /// <exception cref="InsufficientBalanceException"></exception>
    public OrderPlan Plan(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Must be greater than 0", nameof(amount));
        }

        return new OrderPlan(GetPlannedOrders(amount).ToList());
    }

    /// <summary>The order type for the planned orders.</summary>
    protected abstract OrderType OrderType { get; }

    /// <summary>Gets the available balance at the exchange, i.e. Euros when buying, Crypto when selling.</summary>
    /// <param name="availableFunds">The available funds at the exchange.</param>
    /// <returns>The available balance.</returns>
    protected abstract decimal AvailableBalance(AvailableFunds availableFunds);

    /// <summary>Convert a balance (maybe Euro or Crypto) into an amount of crypto product.</summary>
    /// <param name="balance">The available balance.</param>
    /// <param name="price">The price of the product.</param>
    /// <returns>The amount of crypto product represented by the balance.</returns>
    protected abstract decimal BalanceToAmount(decimal balance, decimal price);

    /// <summary>Convert an amount of crypto product into a balance (maybe Euro or Crypto).</summary>
    /// <param name="amount">The amount of crypto product.</param>
    /// <param name="price">The price of the product.</param>
    /// <returns>A balance for the product amount.</returns>
    protected abstract decimal AmountToBalance(decimal amount, decimal price);

    /// <summary>
    /// Gets the order book entries (ask or bid) for an exchange which should be used to fulfill the requested order.
    /// </summary>
    /// <param name="orderBook">The exchange's order book.</param>
    /// <returns>The order book entries.</returns>
    protected abstract IEnumerable<OrderBookEntry> OrderBookEntries(OrderBook orderBook);

    /// <summary>
    /// Sorts the potential order book entries so that the most preferable entries are first,
    /// i.e. lowest price first when buying, highest price first when selling.
    /// </summary>
    /// <param name="entries">The entries to be sorted.</param>
    /// <returns>The sorted entries.</returns>
    protected abstract IEnumerable<OrderPlanState> SortOrderPlanStates(IEnumerable<OrderPlanState> entries);

    /// <summary>
    /// Generates the planned orders for the requested amount.
    /// </summary>
    /// <param name="amount">The amount of crypto product to buy or sell.</param>
    /// <returns>The planned orders to be executed against the exchange.</returns>
    /// <exception cref="InsufficientBalanceException">
    /// Thrown if the user does not have a sufficient balance or there are not enough order book entries
    /// to complete the requested order plan.
    /// </exception>
    private IEnumerable<PlannedOrder> GetPlannedOrders(decimal amount)
    {
        // Flatten the exchange.order structure into a single list of orders
        var orders = _exchangeRepository.Exchanges
            .SelectMany(ex =>
            {
                // The balance on this object is mutable and allows us to track
                // how much has been used at each exchange so far.
                // Each order shares a reference to the same object.
                var exchangeState = new ExchangePlanState
                {
                    ExchangeId = ex.Id,
                    Balance = AvailableBalance(ex.AvailableFunds)
                };

                return OrderBookEntries(ex.OrderBook)
                    .Select(o => new OrderPlanState
                    {
                        ExchangePlanState = exchangeState,
                        OrderId = o.Order.Id,
                        Amount = o.Order.Amount,
                        Price = o.Order.Price
                    });
            });

        // Sort the orders so that the most preferable ones are first.
        // i.e. lowest price first for buy or highest price first for sell.
        orders = SortOrderPlanStates(orders);

        // Take entries from this list until we have fulfilled the requested order
        foreach (var order in orders)
        {
            var plannedAmount = Math.Min(
                Math.Min(order.Amount, amount),
                BalanceToAmount(order.ExchangePlanState.Balance, order.Price));

            if (plannedAmount <= 0)
            {
                // We have run out of funds at this exchange, skip this entry
                continue;
            }

            amount -= plannedAmount;
            order.ExchangePlanState.Balance -= AmountToBalance(plannedAmount, order.Price);

            yield return new PlannedOrder(
                FulfilledByExchangeId: order.ExchangePlanState.ExchangeId,
                FulfilledByOrderId: order.OrderId,
                OrderType: OrderType,
                Amount: plannedAmount,
                Price: order.Price);

            if (amount <= 0)
            {
                // We have completed the requested order, stop
                yield break;
            }
        }

        // We reached the end of the list without completing the requested order
        throw new InsufficientBalanceException();
    }

    /// <summary>
    /// Created per exchange which a mutable balance, to keep track of the remaining available balance during planning.
    /// </summary>
    protected class ExchangePlanState
    {
        /// <summary>The Id of the exchange.</summary>
        public string ExchangeId { get; init; } = string.Empty;

        /// <summary>The current remaining balance.</summary>
        public decimal Balance { get; set; }
    }

    /// <summary>
    /// Intermediate state for an exchange's order book entry used during planning.
    /// </summary>
    protected class OrderPlanState
    {
        /// <summary>Reference to the exchange's remaining balance.</summary>
        public ExchangePlanState ExchangePlanState { get; init; } = new();

        /// <summary>The Id of the order book entry.</summary>
        public Guid OrderId { get; init; }

        /// <summary>The crypto amount of the order book entry.</summary>
        public decimal Amount { get; init; }

        /// <summary>The Euro unit price of the order book entry.</summary>
        public decimal Price { get; init; }
    }
}
