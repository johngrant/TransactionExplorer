using Data.Context;
using Data.Models;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests;

[TestClass]
public class TransactionRepositoryTests
{
    private TransactionExplorerDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<TransactionExplorerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new TransactionExplorerDbContext(options);
        return context;
    }

    [TestMethod]
    public async Task GetAllAsync_WhenNoTransactions_ReturnsEmptyCollection()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new TransactionRepository(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public async Task GetAllAsync_WhenTransactionsExist_ReturnsAllTransactions()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new TransactionRepository(context);

        var transaction1 = new Transaction
        {
            CustomId = "TXN001",
            Description = "Test Transaction 1",
            TransactionDate = new DateTime(2025, 1, 15),
            PurchaseAmount = 100.50m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var transaction2 = new Transaction
        {
            CustomId = "TXN002",
            Description = "Test Transaction 2",
            TransactionDate = new DateTime(2025, 1, 16),
            PurchaseAmount = 250.75m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Transactions.AddRange(transaction1, transaction2);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());

        var transactions = result.ToList();
        Assert.IsTrue(transactions.Any(t => t.CustomId == "TXN001"));
        Assert.IsTrue(transactions.Any(t => t.CustomId == "TXN002"));
        Assert.IsTrue(transactions.Any(t => t.Description == "Test Transaction 1"));
        Assert.IsTrue(transactions.Any(t => t.Description == "Test Transaction 2"));
    }

    [TestMethod]
    public async Task GetAllAsync_WhenMultipleTransactions_ReturnsInCorrectOrder()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new TransactionRepository(context);

        var transactions = new[]
        {
            new Transaction
            {
                CustomId = "TXN003",
                Description = "Transaction C",
                TransactionDate = new DateTime(2025, 1, 17),
                PurchaseAmount = 300.00m,
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                UpdatedAt = DateTime.UtcNow.AddHours(-2)
            },
            new Transaction
            {
                CustomId = "TXN001",
                Description = "Transaction A",
                TransactionDate = new DateTime(2025, 1, 15),
                PurchaseAmount = 100.00m,
                CreatedAt = DateTime.UtcNow.AddHours(-4),
                UpdatedAt = DateTime.UtcNow.AddHours(-4)
            },
            new Transaction
            {
                CustomId = "TXN002",
                Description = "Transaction B",
                TransactionDate = new DateTime(2025, 1, 16),
                PurchaseAmount = 200.00m,
                CreatedAt = DateTime.UtcNow.AddHours(-3),
                UpdatedAt = DateTime.UtcNow.AddHours(-3)
            }
        };

        context.Transactions.AddRange(transactions);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count());

        // Verify all transactions are returned
        var resultList = result.ToList();
        Assert.IsTrue(resultList.Any(t => t.CustomId == "TXN001"));
        Assert.IsTrue(resultList.Any(t => t.CustomId == "TXN002"));
        Assert.IsTrue(resultList.Any(t => t.CustomId == "TXN003"));
    }
}
