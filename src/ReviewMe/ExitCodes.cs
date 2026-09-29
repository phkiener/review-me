namespace ReviewMe;

/// <summary>
/// Well-defined exit codes for the operation.
/// </summary>
public static class ExitCodes
{
    /// <summary>
    /// Signals a successful execution.
    /// </summary>
    public const int Success = 0;

    /// <summary>
    /// Signals an expected or unexpected error.
    /// </summary>
    public const int Error = 1;

    /// <summary>
    /// Signals an error while parsing the commandline arguments.
    /// </summary>
    public const int UnknownArguments = 255;
}
