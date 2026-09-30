namespace ReviewMe.Review;

/// <summary>
/// Categorizes a <see cref="ReviewSuggestion"/> into different buckets.
/// </summary>
public enum SuggestionCategory
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
