using Services.Models;
using System.Globalization;

namespace Services.Utilities;

/// <summary>
/// Utility class for Treasury Exchange Rate API operations
/// </summary>
public static class TreasuryExchangeRateUtilities
{
    /// <summary>
    /// Parses Treasury API date string to DateOnly
    /// </summary>
    /// <param name="dateString">Date string from Treasury API (YYYY-MM-DD format)</param>
    /// <returns>Parsed DateOnly or null if parsing fails</returns>
    public static DateOnly? ParseApiDate(string? dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return null;

        if (DateOnly.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return date;

        return null;
    }

    /// <summary>
    /// Parses Treasury API exchange rate string to decimal
    /// </summary>
    /// <param name="exchangeRateString">Exchange rate string from Treasury API</param>
    /// <returns>Parsed decimal or null if parsing fails</returns>
    public static decimal? ParseApiExchangeRate(string? exchangeRateString)
    {
        if (string.IsNullOrWhiteSpace(exchangeRateString))
            return null;

        if (decimal.TryParse(exchangeRateString, NumberStyles.Number, CultureInfo.InvariantCulture, out var rate))
            return rate;

        return null;
    }

    /// <summary>
    /// Validates that the Treasury API response has valid data
    /// </summary>
    /// <param name="response">Treasury API response</param>
    /// <returns>True if response has valid data, false otherwise</returns>
    public static bool IsValidResponse(TreasuryApiResponse? response)
    {
        return response?.Data != null && response.Data.Any();
    }

    /// <summary>
    /// Gets the most recent exchange rate data from the Treasury API response
    /// </summary>
    /// <param name="response">Treasury API response</param>
    /// <returns>The most recent exchange rate data or null if not found</returns>
    public static TreasuryExchangeRateData? GetMostRecentRate(TreasuryApiResponse response)
    {
        if (!IsValidResponse(response))
            return null;

        return response.Data
            .Where(d => !string.IsNullOrWhiteSpace(d.RecordDate) && !string.IsNullOrWhiteSpace(d.ExchangeRate))
            .OrderByDescending(d => ParseApiDate(d.RecordDate))
            .FirstOrDefault();
    }

    /// <summary>
    /// Formats DateOnly for use in Treasury API queries
    /// </summary>
    /// <param name="date">DateOnly to format</param>
    /// <returns>Formatted date string (YYYY-MM-DD)</returns>
    public static string FormatApiDate(DateOnly date)
    {
        return date.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Builds a currency filter string for Treasury API queries
    /// </summary>
    /// <param name="countryCurrencyDescs">List of currency descriptions</param>
    /// <returns>Formatted filter string</returns>
    public static string BuildCurrencyFilter(IEnumerable<string> countryCurrencyDescs)
    {
        var currencyList = countryCurrencyDescs?.ToList();
        if (currencyList == null || !currencyList.Any())
            throw new ArgumentException("Currency descriptions cannot be null or empty", nameof(countryCurrencyDescs));

        if (currencyList.Count == 1)
            return $"country_currency_desc:eq:{currencyList.First()}";

        return $"country_currency_desc:in:({string.Join(",", currencyList)})";
    }

    /// <summary>
    /// Builds a date range filter string for Treasury API queries
    /// </summary>
    /// <param name="startDate">Start date (optional)</param>
    /// <param name="endDate">End date (optional)</param>
    /// <returns>Formatted filter string or null if no dates provided</returns>
    public static string? BuildDateRangeFilter(DateOnly? startDate, DateOnly? endDate)
    {
        var filters = new List<string>();

        if (startDate.HasValue)
            filters.Add($"record_date:gte:{FormatApiDate(startDate.Value)}");

        if (endDate.HasValue)
            filters.Add($"record_date:lte:{FormatApiDate(endDate.Value)}");

        return filters.Any() ? string.Join(",", filters) : null;
    }
}
