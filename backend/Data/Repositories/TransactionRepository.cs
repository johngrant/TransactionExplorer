using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly TransactionExplorerDbContext _context;

    public TransactionRepository(TransactionExplorerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync()
    {
        return await _context.Transactions.ToListAsync();
    }

    public async Task<Transaction?> GetByIdAsync(int id)
    {
        return await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Transaction?> GetByCustomIdAsync(string customId)
    {
        return await _context.Transactions.FirstOrDefaultAsync(t => t.CustomId == customId);
    }

    public async Task<IEnumerable<Transaction>> GetAsync(
        Expression<Func<Transaction, bool>>? predicate = null,
        Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>>? orderBy = null,
        int? skip = null,
        int? take = null)
    {
        IQueryable<Transaction> query = _context.Transactions;

        // Apply filtering predicate if provided
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        // Apply ordering if provided
        if (orderBy != null)
        {
            query = orderBy(query);
        }

        // Apply paging
        if (skip.HasValue)
        {
            query = query.Skip(skip.Value);
        }

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        return await query.ToListAsync();
    }

    public IAsyncEnumerable<Transaction> StreamAsync(
        Expression<Func<Transaction, bool>>? predicate = null,
        Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>>? orderBy = null,
        int? skip = null,
        int? take = null)
    {
        IQueryable<Transaction> query = _context.Transactions;

        // Apply filtering predicate if provided
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        // Apply ordering if provided
        if (orderBy != null)
        {
            query = orderBy(query);
        }

        // Apply paging
        if (skip.HasValue)
        {
            query = query.Skip(skip.Value);
        }

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        return query.AsAsyncEnumerable();
    }

    public async Task<Transaction> CreateAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }

    public async Task<Transaction> UpdateAsync(Transaction transaction)
    {
        var existingTransaction = await _context.Transactions.FindAsync(transaction.Id);
        if (existingTransaction == null)
        {
            throw new ArgumentException($"Transaction with ID {transaction.Id} not found", nameof(transaction));
        }

        // Update properties
        existingTransaction.CustomId = transaction.CustomId;
        existingTransaction.Description = transaction.Description;
        existingTransaction.TransactionDate = transaction.TransactionDate;
        existingTransaction.PurchaseAmount = transaction.PurchaseAmount;
        existingTransaction.MarkAsUpdated();
        // Keep original CreatedAt

        await _context.SaveChangesAsync();

        return existingTransaction;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction == null)
        {
            return false;
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(string customId)
    {
        return await _context.Transactions.AnyAsync(t => t.CustomId == customId);
    }

    public async Task<int> CountAsync(Expression<Func<Transaction, bool>>? predicate = null)
    {
        IQueryable<Transaction> query = _context.Transactions;

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.CountAsync();
    }
}
