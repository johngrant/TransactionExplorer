using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class ExchangeRateRepository : IExchangeRateRepository
{
    private readonly TransactionExplorerDbContext _context;

    public ExchangeRateRepository(TransactionExplorerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ExchangeRate>> GetAllAsync()
    {
        return await _context.ExchangeRates.ToListAsync();
    }

    public async Task<ExchangeRate?> GetByIdAsync(int id)
    {
        return await _context.ExchangeRates.FindAsync(id);
    }

    public async Task<ExchangeRate?> GetLatestRateAsync(string countryCurrencyDesc, DateOnly transactionDate)
    {
        return await _context.ExchangeRates
            .Where(r => r.CountryCurrencyDesc == countryCurrencyDesc && r.EffectiveDate <= transactionDate)
            .OrderByDescending(r => r.EffectiveDate)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ExchangeRate>> GetRatesForDateRangeAsync(string countryCurrencyDesc, DateOnly startDate, DateOnly endDate)
    {
        return await _context.ExchangeRates
            .Where(r => r.CountryCurrencyDesc == countryCurrencyDesc &&
                       r.EffectiveDate >= startDate &&
                       r.EffectiveDate <= endDate)
            .ToListAsync();
    }

    public async Task<ExchangeRate> CreateAsync(ExchangeRate exchangeRate)
    {
        _context.ExchangeRates.Add(exchangeRate);
        await _context.SaveChangesAsync();
        return exchangeRate;
    }

    public async Task<IEnumerable<ExchangeRate>> CreateBulkAsync(IEnumerable<ExchangeRate> exchangeRates)
    {
        var exchangeRateList = exchangeRates.ToList();
        if (!exchangeRateList.Any())
        {
            return exchangeRateList;
        }

        _context.ExchangeRates.AddRange(exchangeRateList);
        await _context.SaveChangesAsync();
        return exchangeRateList;
    }
}
