using LibGit2Sharp;

namespace ReviewMe.Features;

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
        await host.ReviewAsync(diff.Content);

        return ExitCodes.Success;
    }
}
