using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models;
using WebApi.Models;
using System.Net;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Exchange Rates")]
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
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Exchange rates for the specified period, sorted by date descending</returns>
    /// <response code="200">Returns the exchange rates for the specified period</response>
    /// <response code="400">If the currency description is invalid</response>
    /// <response code="404">If no exchange rates are found for the specified criteria</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("rates")]
    [ProducesResponseType(typeof(TreasuryApiResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<TreasuryApiResponse>> GetExchangeRatesForPeriod(
        [FromQuery] DateOnly transactionDate,
        [FromQuery] string countryCurrencyDesc,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(countryCurrencyDesc))
        {
            return BadRequest("Currency description cannot be null or empty");
        }

        // Calculate the date range: 6 months before the transaction date up to the transaction date
        var startDate = transactionDate.AddMonths(-6);
        var endDate = transactionDate;

        try
        {
            var response = await _treasuryClient.GetExchangeRatesForCurrencyAsync(
                countryCurrencyDesc,
                startDate,
                endDate,
                cancellationToken);

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
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The latest exchange rate for the specified currency</returns>
    /// <response code="200">Returns the latest exchange rate for the specified currency</response>
    /// <response code="400">If the currency description is invalid</response>
    /// <response code="404">If no exchange rate is found for the specified criteria</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("latest")]
    [ProducesResponseType(typeof(TreasuryApiResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<TreasuryApiResponse>> GetLatestExchangeRate(
        [FromQuery] DateOnly transactionDate,
        [FromQuery] string countryCurrencyDesc,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(countryCurrencyDesc))
        {
            return BadRequest("Currency description cannot be null or empty");
        }

        try
        {
            var response = await _treasuryClient.GetLatestExchangeRateAsync(
                countryCurrencyDesc,
                transactionDate,
                cancellationToken);

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
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Conversion result with original amount, exchange rate, and converted amount</returns>
    /// <response code="200">Returns the currency conversion result</response>
    /// <response code="400">If the amount or currency description is invalid</response>
    /// <response code="404">If no exchange rate is found within 6 months of the transaction date</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("convert")]
    [ProducesResponseType(typeof(CurrencyConversionResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<CurrencyConversionResponse>> Convert(
        [FromQuery] DateOnly transactionDate,
        [FromQuery] decimal amountUsd,
        [FromQuery] string countryCurrencyDesc,
        CancellationToken cancellationToken = default)
    {
        if (amountUsd <= 0)
        {
            return BadRequest("Amount must be a positive value");
        }

        if (string.IsNullOrWhiteSpace(countryCurrencyDesc))
        {
            return BadRequest("Currency description cannot be null or empty");
        }

        try
        {
            var response = await _treasuryClient.GetLatestExchangeRateAsync(
                countryCurrencyDesc,
                transactionDate,
                cancellationToken);

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
