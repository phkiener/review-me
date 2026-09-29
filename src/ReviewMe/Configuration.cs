namespace ReviewMe;

public sealed class Configuration
{
    public static string Location => FindConfigurationPath();

    private static string FindConfigurationPath()
    {
        var configHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")
                         ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config");

        return Path.Combine(configHome, "review-me", "settings.json");
    }
}
