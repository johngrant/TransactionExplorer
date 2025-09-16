using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExchangeRateController : ControllerBase
{
    private readonly ITreasuryExchangeRateClient _treasuryClient;

    public ExchangeRateController(ITreasuryExchangeRateClient treasuryClient)
    {
        _treasuryClient = treasuryClient ?? throw new ArgumentNullException(nameof(treasuryClient));
    }

    /// <summary>
    /// Gets exchange rates for a specific currency from 6 months before the transaction date up to the transaction date.
    /// Returns the rates sorted by date (most recent first) to support currency conversion with the most recent rate.
    /// </summary>
    /// <param name="transactionDate">The transaction date</param>
    /// <param name="countryCurrencyDesc">The currency description (e.g., "Canada-Dollar")</param>
    /// <returns>Exchange rates for the specified period, sorted by date descending</returns>
    [HttpGet("rates")]
    public async Task<ActionResult<TreasuryApiResponse>> GetExchangeRatesForPeriod(
        [FromQuery] DateOnly transactionDate,
        [FromQuery] string countryCurrencyDesc)
    {
        if (string.IsNullOrWhiteSpace(countryCurrencyDesc))
        {
            throw new ArgumentException("Currency description cannot be null or empty", nameof(countryCurrencyDesc));
        }

        // Calculate the date range: 6 months before the transaction date up to the transaction date
        var startDate = transactionDate.AddMonths(-6);
        var endDate = transactionDate;

        try
        {
            var response = await _treasuryClient.GetExchangeRatesForCurrencyAsync(
                countryCurrencyDesc,
                startDate,
                endDate);

            if (response.Data == null || !response.Data.Any())
            {
                return NotFound("No exchange rates found for the specified currency and date range.");
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            // Log the exception (in a real application, you'd use ILogger)
            return StatusCode(500, $"An error occurred while retrieving exchange rates: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the latest exchange rate for a specific currency up to the transaction date.
    /// This is useful for getting the most recent rate to use for currency conversion.
    /// </summary>
    /// <param name="transactionDate">The transaction date to find the latest rate for</param>
    /// <param name="countryCurrencyDesc">The currency description (e.g., "Canada-Dollar")</param>
    /// <returns>The latest exchange rate for the specified currency</returns>
    [HttpGet("latest")]
    public async Task<ActionResult<TreasuryApiResponse>> GetLatestExchangeRate(
        [FromQuery] DateOnly transactionDate,
        [FromQuery] string countryCurrencyDesc)
    {
        if (string.IsNullOrWhiteSpace(countryCurrencyDesc))
        {
            throw new ArgumentException("Currency description cannot be null or empty", nameof(countryCurrencyDesc));
        }

        try
        {
            var response = await _treasuryClient.GetLatestExchangeRateAsync(
                countryCurrencyDesc,
                transactionDate);

            if (response.Data == null || !response.Data.Any())
            {
                return NotFound("No exchange rate found for the specified currency and date.");
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            // Log the exception (in a real application, you'd use ILogger)
            return StatusCode(500, $"An error occurred while retrieving exchange rate: {ex.Message}");
        }
    }

    /// <summary>
    /// Converts a USD amount to a target currency using exchange rates from the Treasury API.
    /// Uses the most recent exchange rate available within 6 months of the transaction date.
    /// The converted amount is rounded to two decimal places (cents).
    /// </summary>
    /// <param name="transactionDate">The transaction date</param>
    /// <param name="amountUsd">The amount in USD to convert (must be positive)</param>
    /// <param name="countryCurrencyDesc">The target currency description (e.g., "Canada-Dollar")</param>
    /// <returns>Conversion result with original amount, exchange rate, and converted amount</returns>
    [HttpGet("convert")]
    public async Task<ActionResult<CurrencyConversionResponse>> Convert(
        [FromQuery] DateOnly transactionDate,
        [FromQuery] decimal amountUsd,
        [FromQuery] string countryCurrencyDesc)
    {
        if (amountUsd <= 0)
        {
            throw new ArgumentException("Amount must be a positive value", nameof(amountUsd));
        }

        if (string.IsNullOrWhiteSpace(countryCurrencyDesc))
        {
            throw new ArgumentException("Currency description cannot be null or empty", nameof(countryCurrencyDesc));
        }

        try
        {
            var response = await _treasuryClient.GetLatestExchangeRateAsync(
                countryCurrencyDesc,
                transactionDate);

            if (response.Data == null || !response.Data.Any())
            {
                return NotFound("The purchase cannot be converted to the target currency - no exchange rate found within 6 months.");
            }

            var exchangeRateData = response.Data.First();

            if (!decimal.TryParse(exchangeRateData.ExchangeRate, out var exchangeRate))
            {
                return StatusCode(500, "Invalid exchange rate format received from Treasury API");
            }

            if (!DateOnly.TryParse(exchangeRateData.RecordDate, out var exchangeRateDate))
            {
                return StatusCode(500, "Invalid exchange rate date received from Treasury API");
            }

            // Convert the amount and round to two decimal places (cents)
            var convertedAmount = Math.Round(amountUsd * exchangeRate, 2, MidpointRounding.AwayFromZero);

            var conversionResponse = new CurrencyConversionResponse
            {
                OriginalAmountUsd = amountUsd,
                TransactionDate = transactionDate,
                ExchangeRate = exchangeRate,
                ConvertedAmount = convertedAmount,
                TargetCurrency = countryCurrencyDesc,
                ExchangeRateDate = exchangeRateDate,
                IsExactDateMatch = exchangeRateDate == transactionDate
            };

            return Ok(conversionResponse);
        }
        catch (Exception ex)
        {
            // Log the exception (in a real application, you'd use ILogger)
            return StatusCode(500, $"An error occurred while converting currency: {ex.Message}");
        }
    }
}
