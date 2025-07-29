using CryptoExchangeTask.Core.Model;

namespace CryptoExchangeTask.Core.Repository;

public interface IExchangeRepository
{
    List<Exchange> Exchanges { get; }
}