using System.Text.Json;
using Microsoft.Extensions.Options;
using RestSharp;
using Services.Configuration;
using Services.Interfaces;
using Services.Models;

namespace Services.Clients;

/// <summary>
/// Client for Treasury Reporting Rates of Exchange API
/// </summary>
public class TreasuryExchangeRateClient : ITreasuryExchangeRateClient
{
    private readonly RestClient _restClient;
    private readonly TreasuryExchangeRateOptions _options;
    private const string Endpoint = "v1/accounting/od/rates_of_exchange";

    public TreasuryExchangeRateClient(RestClient restClient, IOptions<TreasuryExchangeRateOptions> options)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
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

        Console.WriteLine($"Treasury API Request: {_restClient.Options.BaseUrl}/{request.Resource}");

        var response = await _restClient.ExecuteAsync<TreasuryApiResponse>(request, cancellationToken);

        Console.WriteLine($"Response Status: {response.StatusCode}");

        // Handle 404 as empty result rather than throwing exception
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.WriteLine("Returning empty response due to 404");
            return new TreasuryApiResponse();
        }

        if (!response.IsSuccessful)
        {
            throw new HttpRequestException($"Request failed with status {response.StatusCode}: {response.ErrorMessage}");
        }

        Console.WriteLine($"Treasury API Response: {response.Content?.Substring(0, Math.Min(500, response.Content?.Length ?? 0))}...");

        Console.WriteLine($"Deserialized Data Count: {response.Data?.Data?.Count ?? 0}");

        return response.Data ?? new TreasuryApiResponse();
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
