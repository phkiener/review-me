namespace ReviewMe.Features;

public sealed class PrintUsage : IFeature
{
    public bool Accepts(string[] args) => args is ["-h"] or ["--help"];

    public Task<int> RunAsync(string[] args)
    {
        Console.WriteLine("Usage");
        Console.WriteLine("  review-me                       Review all uncommitted changes, if there are any, or a diff to the default branch (master/main)");
        Console.WriteLine("  review-me --uncommitted         Review a diff for all uncommitted changes");
        Console.WriteLine("  review-me --diff $REF           Review a diff from the current commit to $REF (can be a branch or a commit hash)");
        Console.WriteLine("  review-me --file $PATH          Review the file at $PATH");
        Console.WriteLine("  review-me config $KEY           Get the current value for $KEY");
        Console.WriteLine("  review-me config $KEY $VALUE    Set $VALUE as config for $KEY");

        return Task.FromResult(ExitCodes.Success);
    }
}
