using LibGit2Sharp;
using ReviewMe.Review;

namespace ReviewMe.Entrypoints.Review;

/// <summary>
/// Reviews all uncommitted changes.
/// </summary>
public sealed class ReviewUncommitted(IReviewProvider reviewProvider, IOutputWriter outputWriter) : IEntrypoint
{
    /// <inheritdoc/>
    public bool Accepts(string[] args) => args is ["--uncommitted"];

    /// <inheritdoc/>
    public async Task<int> RunAsync(string[] args)
    {
        var repositoryRoot = Repository.Discover(Environment.CurrentDirectory);
        var repo = new Repository(repositoryRoot);

        var fullDiff = repo.Diff.Compare<Patch>(null, includeUntracked: true);
        foreach (var diff in fullDiff)
        {
            Console.WriteLine($"Reviewing {diff.Path}...");
            var content = await File.ReadAllTextAsync(diff.Path);

            var change = new FileDiff(diff.Path, content, diff.Patch);
            var suggestions = reviewProvider.GenerateReviewAsync(change, CancellationToken.None);

            await outputWriter.WriteOutputAsync(suggestions, CancellationToken.None);
        }

        return ExitCodes.Success;
    }
}
