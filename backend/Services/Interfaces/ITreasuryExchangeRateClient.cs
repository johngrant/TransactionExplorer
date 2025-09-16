using Services.Models;

namespace Services.Interfaces;

/// <summary>
/// Interface for Treasury Reporting Rates of Exchange API client
/// </summary>
public interface ITreasuryExchangeRateClient
{
    /// <summary>
    /// Retrieves exchange rates from the Treasury API
    /// </summary>
    /// <param name="fields">Comma-separated list of fields to include in response</param>
    /// <param name="filter">Filter criteria for the request</param>
    /// <param name="sort">Sort criteria for the request</param>
    /// <param name="pageNumber">Page number for pagination (default: 1)</param>
    /// <param name="pageSize">Page size for pagination (default: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Treasury API response containing exchange rate data</returns>
    Task<TreasuryApiResponse> GetExchangeRatesAsync(
        string? fields = null,
        string? filter = null,
        string? sort = null,
        int pageNumber = 1,
        int pageSize = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves exchange rates for a specific currency within a date range
    /// </summary>
    /// <param name="countryCurrencyDesc">Currency description (e.g., "Canada-Dollar")</param>
    /// <param name="startDate">Start date for the query</param>
    /// <param name="endDate">End date for the query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Treasury API response containing filtered exchange rate data</returns>
    Task<TreasuryApiResponse> GetExchangeRatesForCurrencyAsync(
        string countryCurrencyDesc,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the latest exchange rate for a specific currency up to a given date
    /// </summary>
    /// <param name="countryCurrencyDesc">Currency description (e.g., "Canada-Dollar")</param>
    /// <param name="asOfDate">The date to find the latest rate for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Treasury API response containing the latest exchange rate data</returns>
    Task<TreasuryApiResponse> GetLatestExchangeRateAsync(
        string countryCurrencyDesc,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves exchange rates for multiple currencies
    /// </summary>
    /// <param name="countryCurrencyDescs">List of currency descriptions</param>
    /// <param name="startDate">Start date for the query</param>
    /// <param name="endDate">End date for the query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Treasury API response containing exchange rate data for multiple currencies</returns>
    Task<TreasuryApiResponse> GetExchangeRatesForMultipleCurrenciesAsync(
        IEnumerable<string> countryCurrencyDescs,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        CancellationToken cancellationToken = default);
}
