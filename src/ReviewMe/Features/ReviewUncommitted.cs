using LibGit2Sharp;
using ReviewMe.Utils;

namespace ReviewMe.Features;

public sealed class ReviewUncommitted : IFeature
{
    public bool Accepts(string[] args) => args is ["--uncommitted"];

    public Task<int> RunAsync(string[] args)
    {
        var repositoryRoot = GitDiscovery.DiscoverRepositoryRoot(Environment.CurrentDirectory);
        var repo = new Repository(repositoryRoot);

        var diff = repo.Diff.Compare<Patch>(null, includeUntracked: true);
        _ = diff;

        return Task.FromResult(ExitCodes.Success);
    }
}
