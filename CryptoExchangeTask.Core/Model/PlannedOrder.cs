namespace CryptoExchangeTask.Core.Model;

public class PlannedOrder
{
    public string FulfilledByExchangeId { get; init; } = string.Empty;

    public Guid FulfilledByOrderId { get; init; }

    public decimal Amount { get; init; }

    public decimal Price { get; init; }
}
