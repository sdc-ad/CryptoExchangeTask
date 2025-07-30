namespace CryptoExchangeTask.Core.Model;

public record Exchange(
    string Id,
    AvailableFunds AvailableFunds,
    OrderBook OrderBook);