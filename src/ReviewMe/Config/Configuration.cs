using Microsoft.Extensions.Configuration;

namespace ReviewMe.Config;

/// <summary>
/// Configuration for <em>ReviewMe</em>.
/// </summary>
/// <param name="Configuration">The underlying <see cref="IConfiguration"/>.</param>
public class Configuration(IConfiguration Configuration)
{
    internal static string Location => FindConfigurationPath();

    /// <summary>
    /// The connection string to the model provider.
    /// </summary>
    public ModelConnectionString? ConnectionString { get; } = Configuration.GetValue<ModelConnectionString>(nameof(ConnectionString));

    private static string FindConfigurationPath()
    {
        var configHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")
                         ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config");


        return Path.Combine(configHome, "review-me", "settings.json");
    }
}
