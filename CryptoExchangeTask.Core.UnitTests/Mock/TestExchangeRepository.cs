using CryptoExchangeTask.Core.Model;
using CryptoExchangeTask.Core.Repository;

namespace CryptoExchangeTask.Core.UnitTests.Mock;

/// <summary>
/// An exchange repository implementation when the set of exchanges is passed in at construction time.
/// </summary>
internal class TestExchangeRepository : IExchangeRepository
{
    public List<Exchange> Exchanges { get; init; } = new();
}
