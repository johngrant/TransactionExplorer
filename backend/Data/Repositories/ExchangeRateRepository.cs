using Data.Context;
using Data.Models;

namespace Data.Repositories;

public class ExchangeRateRepository : IExchangeRateRepository
{
    private readonly TransactionExplorerDbContext _context;

    public ExchangeRateRepository(TransactionExplorerDbContext context)
    {
        _context = context;
    }

    public Task<IEnumerable<ExchangeRate>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ExchangeRate?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ExchangeRate?> GetLatestRateAsync(string countryCurrencyDesc, DateTime transactionDate)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ExchangeRate>> GetRatesForDateRangeAsync(string countryCurrencyDesc, DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }

    public Task<ExchangeRate> CreateAsync(ExchangeRate exchangeRate)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ExchangeRate>> CreateBulkAsync(IEnumerable<ExchangeRate> exchangeRates)
    {
        throw new NotImplementedException();
    }
}
