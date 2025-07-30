namespace CryptoExchangeTask.Core.Model;

/// <summary>
/// The details for a single crypto exchange.
/// </summary>
/// <param name="Id">A string identifier for the exchange.</param>
/// <param name="AvailableFunds">The funds held by the user at the exchange.</param>
/// <param name="OrderBook">The current order book for the exchange.</param>
public record Exchange(
    string Id,
    AvailableFunds AvailableFunds,
    OrderBook OrderBook);