using System.Text.Json;
using System.Threading.Channels;
using ReviewMe.ReviewProviders;

namespace ReviewMe;

public sealed class ConsoleHost : IDisposable
{
    private readonly IReviewProvider reviewProvider = new SimpleOpenAiReviewProvider();
    private Channel<ProgressUpdatedEventArgs>? updates;

    public ConsoleHost()
    {
        reviewProvider.ProgressUpdated += OnProgressUpdated;
    }

    public async Task ReviewAsync(string content)
    {
        updates = Channel.CreateUnbounded<ProgressUpdatedEventArgs>(new() { SingleReader = true, SingleWriter = true });
        var runningReview = Task.Run(() => RunReviewAsync(content));

        var progressUpdates = updates.Reader.ReadAllAsync();
        await foreach (var update in progressUpdates)
        {
            Console.WriteLine(update.Message);
        }

        var suggestions = await runningReview;
        Console.WriteLine(JsonSerializer.Serialize(suggestions, new JsonSerializerOptions { WriteIndented = true }));
    }

    private async Task<IEnumerable<ReviewSuggestion>> RunReviewAsync(string content)
    {
        try
        {
            var result = reviewProvider.GenerateReviewAsync(content, CancellationToken.None);
            return await result.ToListAsync();
        }
        finally
        {
            updates?.Writer.Complete();
        }
    }

    private void OnProgressUpdated(object? sender, ProgressUpdatedEventArgs eventArgs)
    {
        updates?.Writer.TryWrite(eventArgs);
    }

    public void Dispose()
    {
        reviewProvider.ProgressUpdated -= OnProgressUpdated;

        // TODO: Probably DI-wire this up
        if (reviewProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
