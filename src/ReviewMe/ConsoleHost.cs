using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ReviewMe;

public sealed class ConsoleHost(IServiceProvider serviceProvider, ILogger<ConsoleHost> logger)
{
    public async Task ReviewAsync(string content)
    {
        using var reviewProvider = serviceProvider.GetRequiredService<IReviewProvider>();

        var result = await reviewProvider.GenerateReviewAsync(content, CancellationToken.None).ToListAsync(cancellationToken: CancellationToken.None);

        // TODO: Proper output formatting
        Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
    }
}
