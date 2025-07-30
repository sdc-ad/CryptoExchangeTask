namespace CryptoExchangeTask.Core.Model;

public record PlannedOrder(
    string FulfilledByExchangeId,
    Guid FulfilledByOrderId,
    decimal Amount,
    decimal Price);