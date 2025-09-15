using Data.Models;
using System.Linq.Expressions;

namespace Data.Repositories;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task<Transaction?> GetByIdAsync(int id);
    Task<Transaction?> GetByCustomIdAsync(string customId);
    Task<IEnumerable<Transaction>> GetAsync(
        Expression<Func<Transaction, bool>>? predicate = null,
        Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>>? orderBy = null,
        int? skip = null,
        int? take = null);
    IAsyncEnumerable<Transaction> StreamAsync(
        Expression<Func<Transaction, bool>>? predicate = null,
        Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>>? orderBy = null,
        int? skip = null,
        int? take = null);
    Task<Transaction> CreateAsync(Transaction transaction);
    Task<Transaction> UpdateAsync(Transaction transaction);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(string customId);
    Task<int> CountAsync(Expression<Func<Transaction, bool>>? predicate = null);
}
