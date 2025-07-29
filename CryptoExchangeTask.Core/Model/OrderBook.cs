namespace CryptoExchangeTask.Core.Model;
public class OrderBook
{
    public IReadOnlyList<OrderBookEntry> Bids { get; init; } = new List<OrderBookEntry>();

    public IReadOnlyList<OrderBookEntry> Asks { get; init; } = new List<OrderBookEntry>();
}
