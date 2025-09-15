using Data.Models;

namespace Data.Repositories;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task<Transaction?> GetByIdAsync(int id);
    Task<Transaction?> GetByCustomIdAsync(string customId);
    Task<Transaction> CreateAsync(Transaction transaction);
    Task<Transaction> UpdateAsync(Transaction transaction);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(string customId);
}

public interface IExchangeRateRepository
{
    Task<IEnumerable<ExchangeRate>> GetAllAsync();
    Task<ExchangeRate?> GetByIdAsync(int id);
    Task<ExchangeRate?> GetLatestRateAsync(string countryCurrencyDesc, DateTime transactionDate);
    Task<IEnumerable<ExchangeRate>> GetRatesForDateRangeAsync(string countryCurrencyDesc, DateTime startDate, DateTime endDate);
    Task<ExchangeRate> CreateAsync(ExchangeRate exchangeRate);
    Task<IEnumerable<ExchangeRate>> CreateBulkAsync(IEnumerable<ExchangeRate> exchangeRates);
}
