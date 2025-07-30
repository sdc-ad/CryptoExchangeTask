using CryptoExchangeTask.Core.Model;
using System.Reflection;
using System.Text.Json;

namespace CryptoExchangeTask.Core.Repository;

/// <summary>
/// An exchange repository which reads the data from JSON files.
/// </summary>
public class FileExchangeRepository : IExchangeRepository
{
    /// <summary>
    /// Creates an instance of the class. Reads the data from the files here.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the location of the files cannot be determined.</exception>
    public FileExchangeRepository()
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

    /// <inheritdoc/>
    public List<Exchange> Exchanges { get; }
}
