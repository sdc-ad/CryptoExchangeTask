using CryptoExchangeTask.Core.Model;
using Shouldly;
using System.Threading.Tasks;

namespace CryptoExchangeTask.Core.UnitTests;

public class ExchangeRepositoryTests
{
    [Fact]
    public void TestDeserialization()
    {
        var repository = new ExchangeRepository();

        repository.Exchanges.Count.ShouldBe(10);

        var exchange1 = repository.Exchanges.FirstOrDefault(e => e.Id == "exchange-01");
        exchange1.ShouldNotBeNull();

        exchange1.AvailableFunds.ShouldBeEquivalentTo(new AvailableFunds
        {
            Crypto = 10.8503m,
            Euro = 117520.12m
        });

        exchange1.OrderBook.Bids.Count.ShouldBe(100);
        exchange1.OrderBook.Asks.Count.ShouldBe(100);

        exchange1.OrderBook.Bids.First().Order.ShouldBeEquivalentTo(new Order
        {
            Id = new Guid("6e9fe255-a776-4965-9bf4-9f076361f5cb"),
            Amount = 0.01m,
            Price = 57226.46m
        });

        exchange1.OrderBook.Asks.First().Order.ShouldBeEquivalentTo(new Order
        {
            Id = new Guid("719f85c9-163e-471c-8edd-67021cfef195"),
            Amount = 0.405m,
            Price = 57299.73m
        });
    }
}
