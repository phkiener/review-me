using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ReviewMe;

public sealed class ConsoleHost(IServiceProvider serviceProvider, ILogger<ConsoleHost> logger)
{
    public async Task ReviewAsync(string content)
    {
        var progressUpdateChannel = Channel.CreateUnbounded<ProgressUpdatedEventArgs>(new() { SingleReader = true, SingleWriter = true });
        using var reviewProvider = serviceProvider.GetRequiredService<IReviewProvider>();

        var suggestions = await ReviewCoreAsync(reviewProvider, progressUpdateChannel, content);

        // TODO: Proper output formatting
        Console.WriteLine(JsonSerializer.Serialize(suggestions, new JsonSerializerOptions { WriteIndented = true }));
    }

    private async Task<IEnumerable<ReviewSuggestion>> ReviewCoreAsync(IReviewProvider reviewProvider, Channel<ProgressUpdatedEventArgs> progress, string content)
    {
        var reviewTask = Task.Run(() => RunReviewAsync(reviewProvider, content, progress.Writer));

        var progressUpdates = progress.Reader.ReadAllAsync();
        await foreach (var update in progressUpdates)
        {
            logger.LogInformation("{Message}",  update.Message);
        }

        return await reviewTask;
    }

    private static async Task<ReviewSuggestion[]> RunReviewAsync(IReviewProvider reviewProvider, string content, ChannelWriter<ProgressUpdatedEventArgs> progressWriter)
    {
        try
        {
            reviewProvider.ProgressUpdated += (_, update) => progressWriter.TryWrite(update);

            // TODO: Split review per file?
            var result = reviewProvider.GenerateReviewAsync(content, CancellationToken.None);
            return await result.ToArrayAsync();
        }
        finally
        {
            progressWriter.Complete();
        }
    }
}
