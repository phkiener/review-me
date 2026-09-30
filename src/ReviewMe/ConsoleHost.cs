using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReviewMe.Review;

namespace ReviewMe;

public sealed class ConsoleHost(IServiceProvider serviceProvider, ILogger<ConsoleHost> logger)
{
    public async Task ReviewAsync(IReadOnlyList<ReviewRequest> requests)
    {
        using var reviewProvider = serviceProvider.GetRequiredService<IReviewProvider>();

        for (var index = 0; index < requests.Count; index++)
        {
            var request = requests[index];

            logger.LogInformation("Reviewing {File} ({Current}/{Total})", request.FilePath, index + 1, requests.Count);

            var result = await reviewProvider.GenerateReviewAsync(request, CancellationToken.None)
                .ToListAsync(cancellationToken: CancellationToken.None);

            // TODO: Proper output formatting
            Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
