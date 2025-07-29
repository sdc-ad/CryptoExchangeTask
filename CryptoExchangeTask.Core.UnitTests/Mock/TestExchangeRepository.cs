using CryptoExchangeTask.Core.Model;

namespace CryptoExchangeTask.Core.UnitTests.Mock;

internal class TestExchangeRepository : IExchangeRepository
{
    public List<Exchange> Exchanges { get; init; } = new();
}
