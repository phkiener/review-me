using ReviewMe.Review;

namespace ReviewMe;

/// <summary>
/// Writes a stream of <see cref="ReviewSuggestion"/>s to a specific output.
/// </summary>
public interface IOutputWriter
{
    /// <summary>
    /// Render the suggestions.
    /// </summary>
    /// <param name="suggestions">The stream of suggestions to render.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort rendering.</param>
    Task WriteOutputAsync(IAsyncEnumerable<ReviewSuggestion> suggestions, CancellationToken cancellationToken);
}
