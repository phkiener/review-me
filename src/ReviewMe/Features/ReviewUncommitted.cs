using LibGit2Sharp;

namespace ReviewMe.Features;

public sealed class ReviewUncommitted : IFeature
{
    public bool Accepts(string[] args) => args is ["--uncommitted"];

    public async Task<int> RunAsync(string[] args)
    {
        var repositoryRoot = Repository.Discover(Environment.CurrentDirectory);
        var repo = new Repository(repositoryRoot);

        var diff = repo.Diff.Compare<Patch>(null, includeUntracked: true);
        _ = diff;

        var host = new ConsoleHost();
        await host.ReviewAsync(diff.Content);

        return ExitCodes.Success;
    }
}
