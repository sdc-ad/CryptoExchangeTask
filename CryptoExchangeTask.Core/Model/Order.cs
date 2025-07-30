namespace CryptoExchangeTask.Core.Model;

/// <summary>
/// An order in an exchange's order book, may be buy or sell.
/// </summary>
/// <remarks>
/// There are other properties in the JSON which I did not require and have not included in the model.
/// </remarks>
/// <param name="Id">The identifier of the order.</param>
/// <param name="Amount">The crypto amount of the order.</param>
/// <param name="Price">The Euro unit price of the order.</param>
public record Order(
    Guid Id,
    decimal Amount,
    decimal Price);
