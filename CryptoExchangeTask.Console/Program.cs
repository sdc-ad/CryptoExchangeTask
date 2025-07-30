using CryptoExchangeTask.Core;
using CryptoExchangeTask.Core.Errors;
using CryptoExchangeTask.Core.Model;
using CryptoExchangeTask.Core.Repository;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

if (!(args.Length == 2 &&
    Enum.TryParse<OrderType>(args[0], true, out var orderType) &&
    decimal.TryParse(args[1], NumberStyles.AllowDecimalPoint, CultureInfo.CurrentUICulture, out var amount)))
{
    Console.WriteLine($"Usage: CryptoExchangeTask.Console.exe <buy|sell> <amount>");
    return 1;
}

var repository = new FileExchangeRepository();

OrderPlanner planner = orderType switch
{
    OrderType.Buy => new BuyOrderPlanner(repository),
    OrderType.Sell => new SellOrderPlanner(repository),
    _ => throw new ArgumentException("Invalid order type")
};

try
{
    var plan = planner.Plan(amount);
    Console.WriteLine(
        JsonSerializer.Serialize(
            plan,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            }));
}
catch (InsufficientBalanceException)
{
    Console.WriteLine("You do not have enough funds or there are not enough available orders to complete this action");
    return 2;
}

return 0;
