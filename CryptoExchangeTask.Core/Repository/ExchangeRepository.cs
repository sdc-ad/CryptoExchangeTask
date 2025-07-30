using CryptoExchangeTask.Core.Model;
using System.Reflection;
using System.Text.Json;

namespace CryptoExchangeTask.Core.Repository;

public class ExchangeRepository : IExchangeRepository
{
    public ExchangeRepository()
    {
        var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
            throw new InvalidOperationException("Unable to find location of data files");
        Exchanges = Directory.EnumerateFiles(Path.Combine(folder, "Data"))
            .SelectMany(rn =>
            {
                using var stream = File.OpenRead(rn);
                if (stream != null)
                {
                    var exchange = JsonSerializer.Deserialize<Exchange>(stream);

                    if (exchange != null)
                    {
                        return [exchange];
                    }
                }

                return Array.Empty<Exchange>();
            })
            .ToList();
    }

    public List<Exchange> Exchanges { get; }
}
