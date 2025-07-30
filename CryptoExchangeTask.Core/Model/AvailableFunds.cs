namespace CryptoExchangeTask.Core.Model;

/// <summary>
/// The user's available funds at an exchange.
/// </summary>
/// <param name="Crypto">The total held crypto amount.</param>
/// <param name="Euro">The total held Euro amount.</param>
public record AvailableFunds(
    decimal Crypto,
    decimal Euro);