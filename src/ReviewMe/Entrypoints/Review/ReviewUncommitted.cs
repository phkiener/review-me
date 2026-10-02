using LibGit2Sharp;
using ReviewMe.Review;

namespace ReviewMe.Entrypoints.Review;

/// <summary>
/// Reviews all uncommitted changes.
/// </summary>
public sealed class ReviewUncommitted(ConsoleHost host) : IEntrypoint
{
    /// <inheritdoc/>
    public bool Accepts(string[] args) => args is ["--uncommitted"];

    /// <inheritdoc/>
    public async Task<int> RunAsync(string[] args)
    {
        var repositoryRoot = Repository.Discover(Environment.CurrentDirectory);
        var repo = new Repository(repositoryRoot);

        var diff = repo.Diff.Compare<Patch>(null, includeUntracked: true);
        var requests = diff.Select(e => new ReviewRequest(e.Path, e.Patch, IsDiff: true)).ToList();

        await host.ReviewAsync(requests);

        return ExitCodes.Success;
    }
}
