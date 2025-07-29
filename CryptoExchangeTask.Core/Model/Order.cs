namespace CryptoExchangeTask.Core.Model;

public class Order
{
    public Guid Id { get; init; }

    public decimal Amount { get; init; }

    public decimal Price { get; init; }
}
