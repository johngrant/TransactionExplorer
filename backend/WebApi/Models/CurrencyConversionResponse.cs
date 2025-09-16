namespace WebApi.Models;

/// <summary>
/// Response model for currency conversion
/// </summary>
public class CurrencyConversionResponse
{
    /// <summary>
    /// The original amount in USD
    /// </summary>
    public decimal OriginalAmountUsd { get; set; }

    /// <summary>
    /// The transaction date used for the conversion
    /// </summary>
    public DateOnly TransactionDate { get; set; }

    /// <summary>
    /// The exchange rate used for the conversion
    /// </summary>
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// The converted amount in the target currency
    /// </summary>
    public decimal ConvertedAmount { get; set; }

    /// <summary>
    /// The target currency description
    /// </summary>
    public string TargetCurrency { get; set; } = string.Empty;

    /// <summary>
    /// The date of the exchange rate used
    /// </summary>
    public DateOnly ExchangeRateDate { get; set; }

    /// <summary>
    /// Indicates if the exchange rate is from the exact transaction date or an earlier date
    /// </summary>
    public bool IsExactDateMatch { get; set; }
}
