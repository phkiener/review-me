using System.Threading.Channels;

namespace ReviewMe;

public sealed class ConsoleHost : IDisposable
{
    private readonly ReviewSession session = new();
    private readonly Channel<ProgressUpdatedEventArgs> updates = Channel.CreateUnbounded<ProgressUpdatedEventArgs>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });

    private Task? runningReview;

    public ConsoleHost()
    {
        session.ProgressUpdated += OnProgressUpdated;
    }

    public async Task ReviewAsync(string content)
    {
        if (runningReview is not null && !runningReview.IsCompleted)
        {
            throw new InvalidOperationException("Already running a review, mate.");
        }

        runningReview = Task.Run(() => RunReviewAsync(content));

        var progressUpdates = updates.Reader.ReadAllAsync();
        await foreach (var update in progressUpdates)
        {
            Console.WriteLine(update.Message);
        }
    }

    private async Task RunReviewAsync(string content)
    {
        try
        {
            await session.ReviewAsync(content);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            updates.Writer.Complete();
        }
    }

    private void OnProgressUpdated(object? sender, ProgressUpdatedEventArgs eventArgs)
    {
        updates.Writer.TryWrite(eventArgs);
    }

    public void Dispose()
    {
        session.ProgressUpdated -= OnProgressUpdated;
        session.Dispose();
    }
}
