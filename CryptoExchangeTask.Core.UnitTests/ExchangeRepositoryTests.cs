using Shouldly;

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

        exchange1.AvailableFunds.Crypto.ShouldBe(10.8503m);
        exchange1.AvailableFunds.Euro.ShouldBe(117520.12m);
        exchange1.OrderBook.Bids.Count.ShouldBe(100);
        exchange1.OrderBook.Asks.Count.ShouldBe(100);

        var bid = exchange1.OrderBook.Bids.First().Order;
        bid.Id.ShouldBe(new Guid("6e9fe255-a776-4965-9bf4-9f076361f5cb"));
        bid.Amount.ShouldBe(0.01m);
        bid.Price.ShouldBe(57226.46m);

        var ask = exchange1.OrderBook.Asks.First().Order;
        ask.Id.ShouldBe(new Guid("719f85c9-163e-471c-8edd-67021cfef195"));
        ask.Amount.ShouldBe(0.405m);
        ask.Price.ShouldBe(57299.73m);
    }
}
