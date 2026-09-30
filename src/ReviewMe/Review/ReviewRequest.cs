namespace ReviewMe.Review;

/// <summary>
/// A request to review some content.
/// </summary>
/// <param name="FilePath">Path of the file that is to be reviewed.</param>
/// <param name="Content">The content of the file or a diff of the changes to that file.</param>
/// <param name="IsDiff">Whether the content is just a diff or the whole content.</param>
public sealed record ReviewRequest(string FilePath, string Content, bool IsDiff);
