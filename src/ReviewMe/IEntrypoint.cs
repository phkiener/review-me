namespace ReviewMe;

/// <summary>
/// An entrypoint that can be invoked via commandline.
/// </summary>
public interface IEntrypoint
{
    /// <summary>
    /// Checks whether this entrypoint accepts the given arguments, i.e. <see cref="RunAsync"/> may be called.
    /// </summary>
    /// <param name="args">The passed commandline arguments.</param>
    /// <returns><see langword="true"/> if a call to <see cref="RunAsync"/> may proceed, <see langword="false"/> otherwise.</returns>
    bool Accepts(string[] args);

    /// <summary>
    /// Runs the entrypoint with the given <paramref name="args"/>.
    /// </summary>
    /// <param name="args">The commandline arguments.</param>
    /// <returns>An exit code signalling the result of the feature; should be one defined in <see cref="ExitCodes"/>.</returns>
    /// <remarks>
    /// If <see cref="Accepts"/> returns <see langword="false"/> for the given <paramref name="args"/>, the behavior of this method is undefined.
    /// </remarks>
    Task<int> RunAsync(string[] args);
}
