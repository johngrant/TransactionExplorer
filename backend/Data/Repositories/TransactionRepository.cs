using Data.Context;
using Data.Models;

namespace Data.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly TransactionExplorerDbContext _context;

    public TransactionRepository(TransactionExplorerDbContext context)
    {
        _context = context;
    }

    public Task<IEnumerable<Transaction>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Transaction?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Transaction?> GetByCustomIdAsync(string customId)
    {
        throw new NotImplementedException();
    }

    public Task<Transaction> CreateAsync(Transaction transaction)
    {
        throw new NotImplementedException();
    }

    public Task<Transaction> UpdateAsync(Transaction transaction)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(string customId)
    {
        throw new NotImplementedException();
    }
}
