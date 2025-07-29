namespace CryptoExchangeTask.Core.Model;

public class Exchange
{
    public string Id { get; init; } = string.Empty;

    public AvailableFunds AvailableFunds { get; init; } = new AvailableFunds();

    public OrderBook OrderBook { get; init; } = new OrderBook();
}