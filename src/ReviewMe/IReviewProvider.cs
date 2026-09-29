namespace ReviewMe;

/// <summary>
/// An incremental status update for a running call to <see cref="IReviewProvider.GenerateReviewAsync"/>.
/// </summary>
/// <param name="message">A message describing the current progress.</param>
/// <seealso cref="IReviewProvider.ProgressUpdated"/>
public sealed class ProgressUpdatedEventArgs(string message) : EventArgs
{
    /// <summary>
    /// The current progress, described in a human-readable message.
    /// </summary>
    public string Message => message;
}

/// <summary>
/// A review provider capable of generating a list of <see cref="ReviewSuggestion"/>s for a given content.
/// </summary>
public interface IReviewProvider
{
    /// <summary>
    /// An event emitted to signal status updates for a running review.
    /// </summary>
    event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;

    /// <summary>
    /// Generate review comments for the given content.
    /// </summary>
    /// <param name="content">The content to review.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the operation.</param>
    /// <returns>An asynchronous stream of <see cref="ReviewSuggestion"/>s.</returns>
    IAsyncEnumerable<ReviewSuggestion> GenerateReviewAsync(string content, CancellationToken cancellationToken);
}
