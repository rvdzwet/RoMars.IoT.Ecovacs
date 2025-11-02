using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RoMars.IoT.Ecovacs.Interfaces;
using RoMars.IoT.Ecovacs.Services;

namespace RoMars.IoT.Ecovacs.Configuration;

/// <summary>
/// Extension methods for configuring Ecovacs services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Ecovacs client services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddEcovacsClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EcovacsClientOptions>(configuration.GetSection("Ecovacs"));
        return services.AddEcovacsClient();
    }

    /// <summary>
    /// Adds Ecovacs client services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">The options configuration action.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddEcovacsClient(this IServiceCollection services, Action<EcovacsClientOptions> configureOptions)
    {
        services.Configure(configureOptions);
        return services.AddEcovacsClient();
    }

    /// <summary>
    /// Adds Ecovacs client services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddEcovacsClient(this IServiceCollection services)
    {
        services.AddHttpClient("EcovacsClient", (serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<EcovacsClientOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = options.Timeout;
        });

        services.AddScoped<IEcovacsClient, EcovacsClient>();

        return services;
    }
}
