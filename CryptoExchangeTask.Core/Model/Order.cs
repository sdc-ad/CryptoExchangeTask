namespace CryptoExchangeTask.Core.Model;

public record Order(
    Guid Id,
    decimal Amount,
    decimal Price);
