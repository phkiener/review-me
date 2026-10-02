using Microsoft.Extensions.DependencyInjection;
using ReviewMe.Entrypoints.Configuration;
using ReviewMe.Entrypoints.Review;

namespace ReviewMe.Features;

/// <summary>
/// DI registration for all <see cref="IEntrypoint"/>s.
/// </summary>
public static class ServiceProviderConfig
{
    /// <summary>
    /// Add all entrypoints to the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the entrypoints to.</param>
    /// <returns>The given <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddEntrypoints(this IServiceCollection services)
    {
        services.AddSingleton<IEntrypoint, PrintUsage>();
        services.AddSingleton<IEntrypoint, PrintVersion>();
        services.AddSingleton<IEntrypoint, ReviewUncommitted>();
        services.AddSingleton<IEntrypoint, ReviewDiff>();
        services.AddSingleton<IEntrypoint, ReviewFile>();
        services.AddSingleton<IEntrypoint, ReviewAutomatic>();
        services.AddSingleton<IEntrypoint, ConfigurationSetValue>();
        services.AddSingleton<IEntrypoint, ConfigurationGetValue>();
        services.AddSingleton<IEntrypoint, ConfigurationListKeys>();

        // This one *has* to be registered last.
        // It'll match on any arguments, we don't want to skip a possible match.
        services.AddSingleton<IEntrypoint, Fallback>();

        return services;
    }
}
