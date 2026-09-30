namespace ReviewMe.Review;

/// <summary>
/// A review provider capable of generating a list of <see cref="ReviewSuggestion"/>s for a given content.
/// </summary>
public interface IReviewProvider : IDisposable
{
    /// <summary>
    /// Generate review comments for the given content.
    /// </summary>
    /// <param name="request">The content to review.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the operation.</param>
    /// <returns>An asynchronous stream of <see cref="ReviewSuggestion"/>s.</returns>
    IAsyncEnumerable<ReviewSuggestion> GenerateReviewAsync(ReviewRequest request, CancellationToken cancellationToken);
}
