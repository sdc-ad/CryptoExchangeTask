namespace CryptoExchangeTask.Core.Model;

/// <summary>
/// An exchange's order book.
/// </summary>
/// <param name="Bids">The current set of bid orders.</param>
/// <param name="Asks">The current set of ask orders.</param>
public record OrderBook(
    IReadOnlyList<OrderBookEntry> Bids,
    IReadOnlyList<OrderBookEntry> Asks);