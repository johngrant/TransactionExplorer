using System.Text.Json.Serialization;

namespace Services.Models;

/// <summary>
/// Response model for Treasury Reporting Rates of Exchange API
/// </summary>
public class TreasuryApiResponse
{
    [JsonPropertyName("data")]
    public List<TreasuryExchangeRateData> Data { get; set; } = new();

    [JsonPropertyName("meta")]
    public TreasuryApiMeta Meta { get; set; } = new();

    [JsonPropertyName("links")]
    public TreasuryApiLinks Links { get; set; } = new();
}

/// <summary>
/// Exchange rate data from Treasury API
/// </summary>
public class TreasuryExchangeRateData
{
    [JsonPropertyName("record_date")]
    public string RecordDate { get; set; } = string.Empty;

    [JsonPropertyName("country_currency_desc")]
    public string CountryCurrencyDesc { get; set; } = string.Empty;

    [JsonPropertyName("exchange_rate")]
    public string ExchangeRate { get; set; } = string.Empty;

    [JsonPropertyName("effective_date")]
    public string? EffectiveDate { get; set; }
}

/// <summary>
/// Metadata from Treasury API response
/// </summary>
public class TreasuryApiMeta
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("labels")]
    public Dictionary<string, string> Labels { get; set; } = new();

    [JsonPropertyName("dataTypes")]
    public Dictionary<string, string> DataTypes { get; set; } = new();

    [JsonPropertyName("dataFormats")]
    public Dictionary<string, string> DataFormats { get; set; } = new();

    [JsonPropertyName("total-count")]
    public int TotalCount { get; set; }

    [JsonPropertyName("total-pages")]
    public int TotalPages { get; set; }
}

/// <summary>
/// Links object for pagination
/// </summary>
public class TreasuryApiLinks
{
    [JsonPropertyName("self")]
    public string? Self { get; set; }

    [JsonPropertyName("first")]
    public string? First { get; set; }

    [JsonPropertyName("prev")]
    public string? Prev { get; set; }

    [JsonPropertyName("next")]
    public string? Next { get; set; }

    [JsonPropertyName("last")]
    public string? Last { get; set; }
}
