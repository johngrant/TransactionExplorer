using System.Text.Json;
using Services.Interfaces;
using Services.Models;

namespace Services.Clients;

/// <summary>
/// Client for Treasury Reporting Rates of Exchange API
/// </summary>
public class TreasuryExchangeRateClient : ITreasuryExchangeRateClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private const string BaseUrl = "https://api.fiscaldata.treasury.gov/services/api/fiscal_service";
    private const string Endpoint = "/v1/accounting/od/rates_of_exchange";

    public TreasuryExchangeRateClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        // Configure JSON options for case-insensitive property matching
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // Set up base URL if not already configured
        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        // Set common headers
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
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
        var queryParams = new List<string>();

        if (!string.IsNullOrEmpty(fields))
            queryParams.Add($"fields={Uri.EscapeDataString(fields)}");

        if (!string.IsNullOrEmpty(filter))
            queryParams.Add($"filter={Uri.EscapeDataString(filter)}");

        if (!string.IsNullOrEmpty(sort))
            queryParams.Add($"sort={Uri.EscapeDataString(sort)}");

        queryParams.Add($"page[number]={pageNumber}");
        queryParams.Add($"page[size]={pageSize}");

        var queryString = string.Join("&", queryParams);
        var url = $"{Endpoint}?{queryString}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<TreasuryApiResponse>(jsonContent, _jsonOptions);

        return result ?? new TreasuryApiResponse();
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
