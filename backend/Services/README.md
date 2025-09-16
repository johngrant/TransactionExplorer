# Treasury Exchange Rate Client

This document describes how to use the Treasury Exchange Rate Client to fetch exchange rates from the U.S. Treasury Fiscal Data API.

## Overview

The `TreasuryExchangeRateClient` provides a strongly-typed interface to access the Treasury Reporting Rates of Exchange API. This API provides exchange rates for converting between USD and foreign currencies.

## API Endpoint

- **Base URL**: `https://api.fiscaldata.treasury.gov/services/api/fiscal_service`
- **Endpoint**: `/v1/accounting/od/rates_of_exchange`
- **Documentation**: https://fiscaldata.treasury.gov/datasets/treasury-reporting-rates-exchange/

## Configuration

### Using appsettings.json

Add the configuration section to your `appsettings.json`:

```json
{
  "TreasuryExchangeRateApi": {
    "BaseUrl": "https://api.fiscaldata.treasury.gov/services/api/fiscal_service",
    "TimeoutSeconds": 30,
    "DefaultPageSize": 100,
    "MaxRetries": 3
  }
}
```

### Dependency Injection Setup

```csharp
using Services.Extensions;

// In Program.cs or Startup.cs
builder.Services.AddTreasuryExchangeRateClient(builder.Configuration);

// Or with custom options
builder.Services.AddTreasuryExchangeRateClient(options =>
{
    options.BaseUrl = "https://api.fiscaldata.treasury.gov/services/api/fiscal_service";
    options.TimeoutSeconds = 45;
    options.DefaultPageSize = 200;
});
```

## Usage Examples

### Basic Usage

```csharp
using Services.Interfaces;

public class CurrencyService
{
    private readonly ITreasuryExchangeRateClient _treasuryClient;

    public CurrencyService(ITreasuryExchangeRateClient treasuryClient)
    {
        _treasuryClient = treasuryClient;
    }

    public async Task<decimal?> GetExchangeRateAsync(string currency, DateOnly date)
    {
        var response = await _treasuryClient.GetLatestExchangeRateAsync(currency, date);

        if (response.Data.Any())
        {
            var rateData = response.Data.First();
            if (decimal.TryParse(rateData.ExchangeRate, out var rate))
                return rate;
        }

        return null;
    }
}
```

### Get Exchange Rates for Multiple Currencies

```csharp
public async Task<Dictionary<string, decimal>> GetMultipleCurrencyRatesAsync()
{
    var currencies = new[] { "Canada-Dollar", "Euro Zone-Euro", "United Kingdom-Pound" };
    var startDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1));
    var endDate = DateOnly.FromDateTime(DateTime.Now);

    var response = await _treasuryClient.GetExchangeRatesForMultipleCurrenciesAsync(
        currencies, startDate, endDate);

    var rates = new Dictionary<string, decimal>();

    foreach (var data in response.Data)
    {
        if (decimal.TryParse(data.ExchangeRate, out var rate))
        {
            rates[data.CountryCurrencyDesc] = rate;
        }
    }

    return rates;
}
```

### Get Historical Rates for a Specific Currency

```csharp
public async Task<List<(DateOnly Date, decimal Rate)>> GetHistoricalRatesAsync(
    string currency, DateOnly startDate, DateOnly endDate)
{
    var response = await _treasuryClient.GetExchangeRatesForCurrencyAsync(
        currency, startDate, endDate);

    var rates = new List<(DateOnly, decimal)>();

    foreach (var data in response.Data)
    {
        if (DateOnly.TryParseExact(data.RecordDate, "yyyy-MM-dd", out var date) &&
            decimal.TryParse(data.ExchangeRate, out var rate))
        {
            rates.Add((date, rate));
        }
    }

    return rates.OrderByDescending(r => r.Date).ToList();
}
```

## Currency Codes

The Treasury API uses specific currency description formats. Common examples:

- `Canada-Dollar`
- `Euro Zone-Euro`
- `United Kingdom-Pound`
- `Japan-Yen`
- `Australia-Dollar`
- `Mexico-Peso`

## API Response Format

The Treasury API returns data in this structure:

```json
{
  "data": [
    {
      "record_date": "2025-01-15",
      "country_currency_desc": "Canada-Dollar",
      "exchange_rate": "1.25",
      "effective_date": "2025-01-15"
    }
  ],
  "meta": {
    "count": 1,
    "total-count": 1,
    "total-pages": 1
  },
  "links": {
    "self": "...",
    "first": "...",
    "last": "..."
  }
}
```

## Error Handling

The client throws standard HTTP exceptions for API errors. Always wrap calls in try-catch blocks:

```csharp
try
{
    var response = await _treasuryClient.GetLatestExchangeRateAsync("Canada-Dollar", DateOnly.FromDateTime(DateTime.Now));
    // Process response
}
catch (HttpRequestException ex)
{
    // Handle API communication errors
    _logger.LogError(ex, "Failed to retrieve exchange rate from Treasury API");
}
catch (ArgumentException ex)
{
    // Handle invalid parameters
    _logger.LogError(ex, "Invalid parameters provided to Treasury API client");
}
```

## Integration with Existing Models

To convert Treasury API responses to your existing `ExchangeRate` models:

```csharp
using Services.Utilities;
using Data.Models;

public ExchangeRate? ConvertToExchangeRate(TreasuryExchangeRateData apiData)
{
    var recordDate = TreasuryExchangeRateUtilities.ParseApiDate(apiData.RecordDate);
    var effectiveDate = TreasuryExchangeRateUtilities.ParseApiDate(apiData.EffectiveDate);
    var rate = TreasuryExchangeRateUtilities.ParseApiExchangeRate(apiData.ExchangeRate);

    if (!recordDate.HasValue || !rate.HasValue)
        return null;

    return new ExchangeRate
    {
        RecordDate = recordDate.Value,
        CountryCurrencyDesc = apiData.CountryCurrencyDesc,
        ExchangeRateValue = rate.Value,
        EffectiveDate = effectiveDate ?? recordDate.Value,
        CreatedAt = DateTimeOffset.UtcNow
    };
}
```
