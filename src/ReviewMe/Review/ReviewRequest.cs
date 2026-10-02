namespace ReviewMe.Review;

/// <summary>
/// A full file to review.
/// </summary>
/// <param name="FilePath">The path to the file.</param>
/// <param name="Content">The full content of the file.</param>
public sealed record FileContent(string FilePath, string Content);

/// <summary>
/// A diff, i.e. changes to a file, that should be reviewed.
/// </summary>
/// <param name="FilePath">The path to the file.</param>
/// <param name="UpdatedContent">The current full content of the file.</param>
/// <param name="Diff">The diff containing changes done to that file.</param>
public sealed record FileDiff(string FilePath, string UpdatedContent, string Diff);
