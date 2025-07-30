namespace CryptoExchangeTask.Core.Model;
/// <summary>
/// An entry in an exchange's order book.
/// </summary>
/// <param name="Order">The order.</param>
public record OrderBookEntry(
    Order Order);