using Microsoft.Extensions.DependencyInjection;

namespace ReviewMe.Output;

/// <summary>
/// DI registration for <see cref="IOutputWriter"/>s.
/// </summary>
public static class ServiceProviderConfig
{
    /// <summary>
    /// Adds the <see cref="IOutputWriter"/>s to the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The given <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddOutputWriters(this IServiceCollection services)
    {
        services.AddSingleton<IOutputWriter, ConsoleOutputWriter>();

        return services;
    }
}
