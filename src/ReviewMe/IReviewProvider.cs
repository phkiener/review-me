namespace ReviewMe;

public sealed class ProgressUpdatedEventArgs(string message) : EventArgs
{
    public string Message => message;
}

public enum Category { Nitpick, Suggestion, Issue }

public sealed record ReviewSuggestion(string FilePath, int LineNumber, Category Category, string Content);

public interface IReviewProvider
{
    event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;

    IAsyncEnumerable<ReviewSuggestion> GenerateReviewAsync(string content, CancellationToken cancellationToken);
}
