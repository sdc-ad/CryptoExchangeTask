using CryptoExchangeTask.Core.Model;

namespace CryptoExchangeTask.Core;

public class OrderPlanner
{
    private readonly IExchangeRepository _exchangeRepository;

    public OrderPlanner(IExchangeRepository exchangeRepository)
    {
        _exchangeRepository = exchangeRepository;
    }

    public OrderPlan Plan(OrderType type, decimal quantity)
    {
        var exchange = _exchangeRepository.Exchanges.First();
        var order = exchange.OrderBook.Asks.First().Order;

        return new OrderPlan
        {
            PlannedOrders =
            {
                new PlannedOrder
                {
                    FulfilledByExchangeId = exchange.Id,
                    FulfilledByOrderId = order.Id,
                    Amount = quantity,
                    Price = order.Price
                }
            }
        };
    }
}
