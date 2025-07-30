using CryptoExchangeTask.Core.Model;

namespace CryptoExchangeTask.Core.Repository;

/// <summary>
/// A repository which can provide the available exchanges, their order books and the available balance.
/// </summary>
public interface IExchangeRepository
{
    /// <summary>
    /// The available exchanges.
    /// </summary>
    List<Exchange> Exchanges { get; }
}