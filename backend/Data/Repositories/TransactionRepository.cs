using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;

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
}
