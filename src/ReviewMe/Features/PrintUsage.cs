namespace ReviewMe.Features;

/// <summary>
/// Print usage information, i.e. all available features.
/// </summary>
public sealed class PrintUsage : IFeature
{
    /// <inheritdoc/>
    public bool Accepts(string[] args) => args is ["-h"] or ["--help"];

    /// <inheritdoc/>
    public Task<int> RunAsync(string[] args)
    {
        Console.WriteLine("Usage");
        Console.WriteLine("  review-me                       Automatically determine review target; see section below for details");
        Console.WriteLine("  review-me --uncommitted         Review a diff for all uncommitted changes");
        Console.WriteLine("  review-me --diff $REF           Review a diff from the current commit to $REF (can be a branch or a commit hash)");
        Console.WriteLine("  review-me --file $PATH          Review the file at $PATH");
        Console.WriteLine("  review-me config $KEY           Get the current value for $KEY");
        Console.WriteLine("  review-me config $KEY $VALUE    Set $VALUE as config for $KEY");
        Console.WriteLine();
        Console.WriteLine("Automatic review target:");
        Console.WriteLine(" 1) If there are uncommitted changes on the current branch, these are compared (=> --uncommitted)");
        Console.WriteLine(" 2) If HEAD is not master or main, HEAD is compared to the default branch (=> --diff master or --diff main)");
        Console.WriteLine(" 3) Otherwise, HEAD is compared against origin/HEAD (=> --diff origin/master or --diff origin/main)");

        return Task.FromResult(ExitCodes.Success);
    }
}
