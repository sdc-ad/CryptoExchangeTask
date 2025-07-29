using CryptoExchangeTask.Core.Errors;
using CryptoExchangeTask.Core.Model;
using CryptoExchangeTask.Core.Repository;

namespace CryptoExchangeTask.Core;

public abstract class OrderPlanner
{
    private readonly IExchangeRepository _exchangeRepository;

    public OrderPlanner(IExchangeRepository exchangeRepository)
    {
        _exchangeRepository = exchangeRepository;
    }

    public OrderPlan Plan(decimal amount)
    {
        return new OrderPlan
        {
            PlannedOrders = GetPlannedOrders(amount).ToList()
        };
    }

    protected abstract decimal AvailableBalance(AvailableFunds availableFunds);

    protected abstract decimal BalanceToAmount(decimal balance, decimal price);

    protected abstract decimal AmountToBalance(decimal amount, decimal price);

    protected abstract IEnumerable<OrderBookEntry> OrderBookEntries(OrderBook orderBook);

    protected abstract IEnumerable<OrderPlanState> SortOrderPlanStates(IEnumerable<OrderPlanState> entries);

    private IEnumerable<PlannedOrder> GetPlannedOrders(decimal amount)
    {
        var orders = _exchangeRepository.Exchanges
            .SelectMany(ex =>
            {
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

        orders = SortOrderPlanStates(orders);

        foreach (var order in orders)
        {
            var plannedAmount = Math.Min(
                Math.Min(order.Amount, amount),
                BalanceToAmount(order.ExchangePlanState.Balance, order.Price));

            if (plannedAmount <= 0)
            {
                continue;
            }

            amount -= plannedAmount;
            order.ExchangePlanState.Balance -= AmountToBalance(plannedAmount, order.Price);

            yield return new PlannedOrder
            {
                FulfilledByExchangeId = order.ExchangePlanState.ExchangeId,
                FulfilledByOrderId = order.OrderId,
                Amount = plannedAmount,
                Price = order.Price
            };

            if (amount <= 0)
            {
                yield break;
            }
        }

        throw new InsufficientBalanceException();
    }

    protected class ExchangePlanState
    {
        public string ExchangeId { get; init; } = string.Empty;

        public decimal Balance { get; set; }
    }

    protected class OrderPlanState
    {
        public ExchangePlanState ExchangePlanState { get; init; } = new();

        public Guid OrderId { get; init; }

        public decimal Amount { get; init; }

        public decimal Price { get; init; }
    }
}
