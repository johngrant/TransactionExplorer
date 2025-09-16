using Data.Models;

namespace Data.Repositories;

public interface IExchangeRateRepository
{
    Task<IEnumerable<ExchangeRate>> GetAllAsync();
    Task<ExchangeRate?> GetByIdAsync(int id);
    Task<ExchangeRate?> GetLatestRateAsync(string countryCurrencyDesc, DateOnly transactionDate);
    Task<IEnumerable<ExchangeRate>> GetRatesForDateRangeAsync(string countryCurrencyDesc, DateOnly startDate, DateOnly endDate);
    Task<ExchangeRate> CreateAsync(ExchangeRate exchangeRate);
    Task<IEnumerable<ExchangeRate>> CreateBulkAsync(IEnumerable<ExchangeRate> exchangeRates);
}
