using CryptoExchangeTask.Core.Model;

namespace CryptoExchangeTask.Core;

public interface IExchangeRepository
{
    List<Exchange> Exchanges { get; }
}