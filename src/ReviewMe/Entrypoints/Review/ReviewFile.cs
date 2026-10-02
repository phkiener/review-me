using ReviewMe.Review;

namespace ReviewMe.Entrypoints.Review;

/// <summary>
/// Reviews a specific file.
/// </summary>
public sealed class ReviewFile(IReviewProvider reviewProvider, IOutputWriter outputWriter) : IEntrypoint
{
    /// <inheritdoc/>
    public bool Accepts(string[] args) => args is ["--file", _];

    /// <inheritdoc/>
    public async Task<int> RunAsync(string[] args)
    {
        var filePath = args[1];

        if (!File.Exists(filePath))
        {
            return ExitCodes.Error;
        }

        var content = await File.ReadAllTextAsync(args[1]);
        var fileContent = new FileContent(args[1], content);

        Console.WriteLine($"Reviewing {args[1]}...");
        var suggestions = reviewProvider.GenerateReviewAsync(fileContent, CancellationToken.None);
        await outputWriter.WriteOutputAsync(suggestions, CancellationToken.None);

        return ExitCodes.Success;
    }
}
