namespace Services.Configuration;

/// <summary>
/// Configuration options for Treasury Exchange Rate API client
/// </summary>
public class TreasuryExchangeRateOptions
{
    public const string SectionName = "TreasuryExchangeRateApi";

    /// <summary>
    /// Base URL for the Treasury API (default: https://api.fiscaldata.treasury.gov/services/api/fiscal_service)
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.fiscaldata.treasury.gov/services/api/fiscal_service";

    /// <summary>
    /// Default timeout for HTTP requests in seconds (default: 30)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Default page size for API requests (default: 100)
    /// </summary>
    public int DefaultPageSize { get; set; } = 100;

    /// <summary>
    /// Maximum number of retries for failed requests (default: 3)
    /// </summary>
    public int MaxRetries { get; set; } = 3;
}
