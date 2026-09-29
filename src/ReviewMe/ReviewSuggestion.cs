namespace ReviewMe;

/// <summary>
/// Categorizes a <see cref="ReviewSuggestion"/> into different buckets.
/// </summary>
public enum Category
{
    /// <summary>
    /// A stylistic choice that might be done differently, a typo or other small issues.
    /// </summary>
    Nitpick,

    /// <summary>
    /// A mere suggestion that may be safely ignored.
    /// </summary>
    Suggestion,

    /// <summary>
    /// An issue that should be addressed.
    /// </summary>
    Issue
}

/// <summary>
/// A single suggestion for a line of code.
/// </summary>
/// <param name="FilePath">File for which this suggestion applies.</param>
/// <param name="LineNumber">Line number in the file given by <see cref="FilePath"/>.</param>
/// <param name="Category">The <see cref="Category"/> of the comment.</param>
/// <param name="Content">The content of the comment.</param>
public sealed record ReviewSuggestion(string FilePath, int LineNumber, Category Category, string Content);
