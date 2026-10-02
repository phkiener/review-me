using LibGit2Sharp;
using ReviewMe.Review;

namespace ReviewMe.Entrypoints.Review;

/// <summary>
/// Reviews the code against a given reference (tag, branch or commit).
/// </summary>
public sealed class ReviewDiff(IReviewProvider reviewProvider, IOutputWriter outputWriter) : IEntrypoint
{
    /// <inheritdoc/>
    public bool Accepts(string[] args) => args is ["--diff", _];

    /// <inheritdoc/>
    public async Task<int> RunAsync(string[] args)
    {
        var repositoryRoot = Repository.Discover(Environment.CurrentDirectory);
        var repo = new Repository(repositoryRoot);

        var oldTree = GetCompareTarget(repo, args[1]);
        var newTree = repo.Head.Tip.Tree;

        if (oldTree is null)
        {
            return ExitCodes.Error;
        }

        var fullDiff = repo.Diff.Compare<Patch>(oldTree, newTree);
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

    private static Tree? GetCompareTarget(Repository repository, string reference)
    {
        var matchingTag = repository.Tags.FirstOrDefault(t => t.FriendlyName == reference);
        if (matchingTag is not null)
        {
            var commitId = matchingTag.Target.Id;
            var foundCommit = repository.Commits.Single(c => c.Id == commitId);

            return foundCommit.Tree;
        }

        var matchingBranch = repository.Branches.FirstOrDefault(b => b.FriendlyName == reference);
        if (matchingBranch is not null)
        {
            return matchingBranch.Tip.Tree;
        }

        var matchingCommit = repository.Commits.FirstOrDefault(c => c.Sha.StartsWith(reference));
        return matchingCommit?.Tree;
    }
}
