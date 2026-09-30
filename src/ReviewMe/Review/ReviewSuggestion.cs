namespace ReviewMe.Review;

/// <summary>
/// A single suggestion for a line of code.
/// </summary>
/// <param name="FilePath">File for which this suggestion applies.</param>
/// <param name="LineNumber">Line number in the file given by <see cref="FilePath"/>.</param>
/// <param name="Category">The <see cref="Category"/> of the comment.</param>
/// <param name="Content">The content of the comment.</param>
public sealed record ReviewSuggestion(string FilePath, int LineNumber, SuggestionCategory Category, string Content);
