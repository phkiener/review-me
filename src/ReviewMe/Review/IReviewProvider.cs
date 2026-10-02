namespace ReviewMe.Review;

/// <summary>
/// A review provider capable of generating a list of <see cref="ReviewSuggestion"/>s for a given content.
/// </summary>
public interface IReviewProvider : IDisposable
{
    /// <summary>
    /// Generate review comments for the given file.
    /// </summary>
    /// <param name="fileContent">The file to review.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the operation.</param>
    /// <returns>An asynchronous stream of <see cref="ReviewSuggestion"/>s.</returns>
    IAsyncEnumerable<ReviewSuggestion> GenerateReviewAsync(FileContent fileContent, CancellationToken cancellationToken);

    /// <summary>
    /// Generate review comments for the given diff.
    /// </summary>
    /// <param name="diff">The diff to review.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the operation.</param>
    /// <returns>An asynchronous stream of <see cref="ReviewSuggestion"/>s.</returns>
    IAsyncEnumerable<ReviewSuggestion> GenerateReviewAsync(FileDiff diff, CancellationToken cancellationToken);
}
