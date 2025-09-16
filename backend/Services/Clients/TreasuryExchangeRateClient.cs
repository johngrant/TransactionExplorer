using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RestSharp;
using Services.Configuration;
using Services.Interfaces;
using Services.Models;

namespace Services.Clients;

/// <summary>
/// Client for Treasury Reporting Rates of Exchange API with retry policies
/// </summary>
public class TreasuryExchangeRateClient : ITreasuryExchangeRateClient
{
    private readonly IRestClientWrapper _restClientWrapper;
    private readonly TreasuryExchangeRateOptions _options;
    private readonly ILogger<TreasuryExchangeRateClient> _logger;
    private readonly ResiliencePipeline<IRestResponseWrapper> _retryPipeline;
    private const string Endpoint = "v1/accounting/od/rates_of_exchange";

    public TreasuryExchangeRateClient(
        IRestClientWrapper restClientWrapper, 
        IOptions<TreasuryExchangeRateOptions> options,
        ILogger<TreasuryExchangeRateClient> logger)
    {
        _restClientWrapper = restClientWrapper ?? throw new ArgumentNullException(nameof(restClientWrapper));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Configure retry policy
        _retryPipeline = new ResiliencePipelineBuilder<IRestResponseWrapper>()
            .AddRetry(new Polly.Retry.RetryStrategyOptions<IRestResponseWrapper>
            {
                ShouldHandle = new PredicateBuilder<IRestResponseWrapper>()
                    .Handle<HttpRequestException>()
                    .Handle<TaskCanceledException>()
                    .HandleResult(response => 
                        !response.IsSuccessful && 
                        response.StatusCode != HttpStatusCode.NotFound && // Don't retry 404s
                        response.StatusCode != HttpStatusCode.BadRequest), // Don't retry bad requests
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                UseJitter = true,
                OnRetry = args =>
                {
                    _logger.LogWarning("Treasury API retry attempt {AttemptNumber} after {Delay}ms. Outcome: {Outcome}", 
                        args.AttemptNumber, 
                        args.RetryDelay.TotalMilliseconds,
                        args.Outcome);
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    /// <inheritdoc />
    public async Task<TreasuryApiResponse> GetExchangeRatesAsync(
        string? fields = null,
        string? filter = null,
        string? sort = null,
        int pageNumber = 1,
        int pageSize = 100,
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Endpoint, Method.Get);

        if (!string.IsNullOrEmpty(fields))
            request.AddQueryParameter("fields", fields);

        if (!string.IsNullOrEmpty(filter))
            request.AddQueryParameter("filter", filter);

        if (!string.IsNullOrEmpty(sort))
            request.AddQueryParameter("sort", sort);

        request.AddQueryParameter("page[number]", pageNumber.ToString());
        request.AddQueryParameter("page[size]", pageSize.ToString());

        _logger.LogDebug("Treasury API Request: {BaseUrl}/{Resource}", _restClientWrapper.BaseUrl, request.Resource);

        // Execute with retry policy
        var response = await _retryPipeline.ExecuteAsync(async (cancellationToken) =>
        {
            return await _restClientWrapper.ExecuteAsync(request, cancellationToken);
        }, cancellationToken);

        _logger.LogDebug("Response Status: {StatusCode}", response.StatusCode);

        // Handle 404 as empty result rather than throwing exception
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("No exchange rate data found (404) - returning empty response");
            return new TreasuryApiResponse();
        }

        if (!response.IsSuccessful)
        {
            _logger.LogError("Treasury API request failed with status {StatusCode}: {ErrorMessage}", 
                response.StatusCode, response.ErrorMessage);
            throw new HttpRequestException($"Request failed with status {response.StatusCode}: {response.ErrorMessage}");
        }

        _logger.LogDebug("Treasury API Response: {ResponsePreview}...", 
            response.Content?.Substring(0, Math.Min(500, response.Content?.Length ?? 0)));

        var treasuryResponse = JsonSerializer.Deserialize<TreasuryApiResponse>(response.Content ?? "{}");
        _logger.LogDebug("Deserialized Data Count: {DataCount}", treasuryResponse?.Data?.Count ?? 0);

        return treasuryResponse ?? new TreasuryApiResponse();
    }

    /// <inheritdoc />
    public async Task<TreasuryApiResponse> GetExchangeRatesForCurrencyAsync(
        string countryCurrencyDesc,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(countryCurrencyDesc))
            throw new ArgumentException("Currency description cannot be null or empty", nameof(countryCurrencyDesc));

        var filters = new List<string>
        {
            $"country_currency_desc:eq:{countryCurrencyDesc}"
        };

        if (startDate.HasValue)
            filters.Add($"record_date:gte:{startDate.Value:yyyy-MM-dd}");

        if (endDate.HasValue)
            filters.Add($"record_date:lte:{endDate.Value:yyyy-MM-dd}");

        var filter = string.Join(",", filters);
        const string fields = "record_date,country_currency_desc,exchange_rate,effective_date";
        const string sort = "-record_date"; // Sort by most recent first

        return await GetExchangeRatesAsync(fields, filter, sort, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TreasuryApiResponse> GetLatestExchangeRateAsync(
        string countryCurrencyDesc,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(countryCurrencyDesc))
            throw new ArgumentException("Currency description cannot be null or empty", nameof(countryCurrencyDesc));

        // Get rates for the currency up to the specified date, sorted by most recent first
        var sixMonthsEarlier = asOfDate.AddMonths(-6);
        var filter = $"country_currency_desc:eq:{countryCurrencyDesc},record_date:lte:{asOfDate:yyyy-MM-dd},record_date:gte:{sixMonthsEarlier:yyyy-MM-dd}";
        const string fields = "record_date,country_currency_desc,exchange_rate,effective_date";
        const string sort = "-record_date"; // Sort by most recent first

        return await GetExchangeRatesAsync(fields, filter, sort, pageSize: 1, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TreasuryApiResponse> GetExchangeRatesForMultipleCurrenciesAsync(
        IEnumerable<string> countryCurrencyDescs,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var currencyList = countryCurrencyDescs?.ToList();
        if (currencyList == null || !currencyList.Any())
            throw new ArgumentException("Currency descriptions cannot be null or empty", nameof(countryCurrencyDescs));

        var currencyFilter = $"country_currency_desc:in:({string.Join(",", currencyList)})";
        var filters = new List<string> { currencyFilter };

        if (startDate.HasValue)
            filters.Add($"record_date:gte:{startDate.Value:yyyy-MM-dd}");

        if (endDate.HasValue)
            filters.Add($"record_date:lte:{endDate.Value:yyyy-MM-dd}");

        var filter = string.Join(",", filters);
        const string fields = "record_date,country_currency_desc,exchange_rate,effective_date";
        const string sort = "-record_date,country_currency_desc"; // Sort by date desc, then currency

        return await GetExchangeRatesAsync(fields, filter, sort, pageSize: 1000, cancellationToken: cancellationToken);
    }
}
