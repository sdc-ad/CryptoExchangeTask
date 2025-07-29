using CryptoExchangeTask.Core.Model;
using CryptoExchangeTask.Core.Repository;

namespace CryptoExchangeTask.Core.UnitTests.Mock;

internal class TestExchangeRepository : IExchangeRepository
{
    public List<Exchange> Exchanges { get; init; } = new();
}
