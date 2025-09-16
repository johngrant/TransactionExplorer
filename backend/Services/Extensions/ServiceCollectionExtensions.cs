using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RestSharp;
using Services.Clients;
using Services.Configuration;
using Services.Interfaces;

namespace Services.Extensions;

/// <summary>
/// Extension methods for configuring services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Treasury Exchange Rate client services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Configuration instance</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddTreasuryExchangeRateClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure options
        services.Configure<TreasuryExchangeRateOptions>(
            configuration.GetSection(TreasuryExchangeRateOptions.SectionName));

        // Register RestClient as a singleton with proper configuration
        services.AddSingleton<RestClient>(serviceProvider =>
        {
            var options = configuration.GetSection(TreasuryExchangeRateOptions.SectionName)
                                    .Get<TreasuryExchangeRateOptions>() ?? new TreasuryExchangeRateOptions();

            var clientOptions = new RestClientOptions(options.BaseUrl)
            {
                Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds)
            };

            return new RestClient(clientOptions);
        });

        // Register the client service
        services.AddScoped<ITreasuryExchangeRateClient, TreasuryExchangeRateClient>();

        return services;
    }

    /// <summary>
    /// Adds Treasury Exchange Rate client services with custom configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Action to configure options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddTreasuryExchangeRateClient(
        this IServiceCollection services,
        Action<TreasuryExchangeRateOptions> configureOptions)
    {
        var options = new TreasuryExchangeRateOptions();
        configureOptions(options);

        services.Configure<TreasuryExchangeRateOptions>(opt =>
        {
            opt.BaseUrl = options.BaseUrl;
            opt.TimeoutSeconds = options.TimeoutSeconds;
            opt.DefaultPageSize = options.DefaultPageSize;
            opt.MaxRetries = options.MaxRetries;
        });

        // Register RestClient as a singleton with proper configuration
        services.AddSingleton<RestClient>(serviceProvider =>
        {
            var clientOptions = new RestClientOptions(options.BaseUrl)
            {
                Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds)
            };

            return new RestClient(clientOptions);
        });

        // Register the client service
        services.AddScoped<ITreasuryExchangeRateClient, TreasuryExchangeRateClient>();

        return services;
    }
}
