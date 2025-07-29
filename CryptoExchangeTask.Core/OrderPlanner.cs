using CryptoExchangeTask.Core.Errors;
using CryptoExchangeTask.Core.Model;

namespace CryptoExchangeTask.Core;

public class OrderPlanner
{
    private readonly IExchangeRepository _exchangeRepository;

    public OrderPlanner(IExchangeRepository exchangeRepository)
    {
        _exchangeRepository = exchangeRepository;
    }

    public OrderPlan Plan(OrderType type, decimal amount)
    {
        return new OrderPlan
        {
            PlannedOrders = GetPlannedOrders(type, amount).ToList()
        };
    }

    private IEnumerable<PlannedOrder> GetPlannedOrders(OrderType type, decimal amount)
    {
        var orders = _exchangeRepository.Exchanges
            .SelectMany(ex =>
            {
                var exchangeState = new ExchangePlanState
                {
                    ExchangeId = ex.Id,
                    Balance = ex.AvailableFunds.Euro
                };

                return ex.OrderBook.Asks
                    .Select(o => new
                    {
                        ExchangeState = exchangeState,
                        OrderId = o.Order.Id,
                        Amount = o.Order.Amount,
                        Price = o.Order.Price
                    });
            })
            .OrderBy(o => o.Price);

        foreach (var order in orders)
        {
            var plannedAmount = Math.Min(Math.Min(order.Amount, amount), order.ExchangeState.Balance / order.Price);

            if (plannedAmount <= 0)
            {
                continue;
            }

            amount -= plannedAmount;
            order.ExchangeState.Balance -= plannedAmount;

            yield return new PlannedOrder
            {
                FulfilledByExchangeId = order.ExchangeState.ExchangeId,
                FulfilledByOrderId = order.OrderId,
                Amount = plannedAmount,
                Price = order.Price
            };

            if (amount <= 0)
            {
                break;
            }
        }

        if (amount > 0)
        {
            throw new InsufficientBalanceException();
        }
    }

    private class ExchangePlanState
    {
        public string ExchangeId { get; init; } = string.Empty;

        public decimal Balance { get; set; }
    }
}
