using Data.Models;

namespace Data.Repositories;

public interface IExchangeRateRepository
{
    Task<IEnumerable<ExchangeRate>> GetAllAsync();
    Task<ExchangeRate?> GetByIdAsync(int id);
    Task<ExchangeRate?> GetLatestRateAsync(string countryCurrencyDesc, DateTime transactionDate);
    Task<IEnumerable<ExchangeRate>> GetRatesForDateRangeAsync(string countryCurrencyDesc, DateTime startDate, DateTime endDate);
    Task<ExchangeRate> CreateAsync(ExchangeRate exchangeRate);
    Task<IEnumerable<ExchangeRate>> CreateBulkAsync(IEnumerable<ExchangeRate> exchangeRates);
}
