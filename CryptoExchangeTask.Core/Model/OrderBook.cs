namespace CryptoExchangeTask.Core.Model;
public class OrderBook
{
    public List<OrderBookEntry> Bids { get; init; } = new();

    public List<OrderBookEntry> Asks { get; init; } = new();
}
