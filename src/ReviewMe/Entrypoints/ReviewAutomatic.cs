using LibGit2Sharp;

namespace ReviewMe.Features;

/// <summary>
/// Reviews the code with an automagically chosen target.
/// </summary>
public sealed class ReviewAutomatic : IEntrypoint
{
    /// <inheritdoc/>
    public bool Accepts(string[] args) => args is [];

    /// <inheritdoc/>
    public Task<int> RunAsync(string[] args)
    {
        var repositoryRoot = Repository.Discover(Environment.CurrentDirectory);
        var repo = new Repository(repositoryRoot);

        if (repo.RetrieveStatus().IsDirty)
        {
            var uncommitted = new ReviewUncommitted();
            return uncommitted.RunAsync(["--uncommitted"]);
        }

        var defaultBranch = repo.Branches["master"] ?? repo.Branches["main"];
        if (defaultBranch is not null)
        {
            var currentBranch = repo.Head;
            var targetBranch = currentBranch.FriendlyName == defaultBranch.FriendlyName
                ? "origin/" + currentBranch.FriendlyName
                : defaultBranch.FriendlyName;

            var diff = new ReviewDiff();
            return diff.RunAsync(["--diff", targetBranch]);
        }

        return Task.FromResult(ExitCodes.Success);
    }
}
