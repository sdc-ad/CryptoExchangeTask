namespace CryptoExchangeTask.Core.Model;

public record OrderBook(
    IReadOnlyList<OrderBookEntry> Bids,
    IReadOnlyList<OrderBookEntry> Asks);