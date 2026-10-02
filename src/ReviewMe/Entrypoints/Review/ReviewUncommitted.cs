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
            if (diff.Status is ChangeKind.Deleted or ChangeKind.Renamed)
            {
                Console.WriteLine($"Skipping {diff.Path} (deleted and renamed files are not reviewed)...");
                continue;
            }

            Console.WriteLine($"Reviewing {diff.Path}...");
            var content = await File.ReadAllTextAsync(diff.Path);

            var change = new FileDiff(diff.Path, content, diff.Patch);
            var suggestions = reviewProvider.GenerateReviewAsync(change, CancellationToken.None);

            await outputWriter.WriteOutputAsync(suggestions, CancellationToken.None);
        }

        Console.WriteLine("Review completed. Note: You shouldn't blindly trust LLM-generated text.");
        Console.WriteLine("Take the generated comments with a grain of salt. They might be correct, they might be technically correct - or they might be absolutely wrong.");

        return ExitCodes.Success;
    }
}
