using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
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

        // Register HttpClient for TreasuryExchangeRateClient
        services.AddHttpClient<ITreasuryExchangeRateClient, TreasuryExchangeRateClient>((serviceProvider, httpClient) =>
        {
            var options = configuration.GetSection(TreasuryExchangeRateOptions.SectionName)
                                    .Get<TreasuryExchangeRateOptions>() ?? new TreasuryExchangeRateOptions();

            httpClient.BaseAddress = new Uri(options.BaseUrl);
            httpClient.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);

            // Set user agent
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TransactionExplorer/1.0");

            // Set default headers
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        });

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

        // Register HttpClient for TreasuryExchangeRateClient
        services.AddHttpClient<ITreasuryExchangeRateClient, TreasuryExchangeRateClient>((serviceProvider, httpClient) =>
        {
            httpClient.BaseAddress = new Uri(options.BaseUrl);
            httpClient.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);

            // Set user agent
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TransactionExplorer/1.0");

            // Set default headers
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        });

        return services;
    }
}
