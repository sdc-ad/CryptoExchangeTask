namespace CryptoExchangeTask.Core.Model;

public class Order
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public decimal Amount { get; init; }

    public decimal Price { get; init; }
}
