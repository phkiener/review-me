namespace ReviewMe.Utils;

public static class GitDiscovery
{
    public static string? DiscoverRepositoryRoot(string startingDirectory)
    {
        var currentDirectory = startingDirectory;

        while (currentDirectory is not null && !Directory.Exists(Path.Combine(currentDirectory, ".git")))
        {
            currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
        }

        return currentDirectory;
    }
}
