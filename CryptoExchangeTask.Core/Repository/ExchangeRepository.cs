using CryptoExchangeTask.Core.Model;
using System.Text.Json;

namespace CryptoExchangeTask.Core.Repository;

public class ExchangeRepository : IExchangeRepository
{
    public ExchangeRepository()
    {
        var assembly = GetType().Assembly;

        Exchanges = assembly
            .GetManifestResourceNames()
            .SelectMany(rn =>
            {
                using var stream = assembly.GetManifestResourceStream(rn);
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
