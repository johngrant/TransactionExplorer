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

    [TestMethod]
    public async Task GetByIdAsync_WhenTransactionExists_ReturnsTransaction()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new TransactionRepository(context);

        var transaction = new Transaction
        {
            CustomId = "TXN001",
            Description = "Test Transaction",
            TransactionDate = new DateTime(2025, 1, 15),
            PurchaseAmount = 100.50m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(transaction.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(transaction.Id, result.Id);
        Assert.AreEqual("TXN001", result.CustomId);
        Assert.AreEqual("Test Transaction", result.Description);
        Assert.AreEqual(new DateTime(2025, 1, 15), result.TransactionDate);
        Assert.AreEqual(100.50m, result.PurchaseAmount);
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenTransactionDoesNotExist_ReturnsNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new TransactionRepository(context);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenMultipleTransactionsExist_ReturnsCorrectOne()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new TransactionRepository(context);

        var transaction1 = new Transaction
        {
            CustomId = "TXN001",
            Description = "First Transaction",
            TransactionDate = new DateTime(2025, 1, 15),
            PurchaseAmount = 100.50m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var transaction2 = new Transaction
        {
            CustomId = "TXN002",
            Description = "Second Transaction",
            TransactionDate = new DateTime(2025, 1, 16),
            PurchaseAmount = 250.75m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Transactions.AddRange(transaction1, transaction2);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(transaction2.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(transaction2.Id, result.Id);
        Assert.AreEqual("TXN002", result.CustomId);
        Assert.AreEqual("Second Transaction", result.Description);
        Assert.AreNotEqual(transaction1.Id, result.Id);
    }

    [TestMethod]
    public async Task GetByCustomIdAsync_WhenTransactionExists_ReturnsTransaction()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new TransactionRepository(context);

        var transaction = new Transaction
        {
            CustomId = "TXN001",
            Description = "Test Transaction",
            TransactionDate = new DateTime(2025, 1, 15),
            PurchaseAmount = 100.50m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByCustomIdAsync("TXN001");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("TXN001", result.CustomId);
        Assert.AreEqual("Test Transaction", result.Description);
        Assert.AreEqual(new DateTime(2025, 1, 15), result.TransactionDate);
        Assert.AreEqual(100.50m, result.PurchaseAmount);
    }

    [TestMethod]
    public async Task GetByCustomIdAsync_WhenTransactionDoesNotExist_ReturnsNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new TransactionRepository(context);

        // Act
        var result = await repository.GetByCustomIdAsync("NONEXISTENT");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetByCustomIdAsync_WhenMultipleTransactionsExist_ReturnsCorrectOne()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new TransactionRepository(context);

        var transaction1 = new Transaction
        {
            CustomId = "TXN001",
            Description = "First Transaction",
            TransactionDate = new DateTime(2025, 1, 15),
            PurchaseAmount = 100.50m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var transaction2 = new Transaction
        {
            CustomId = "TXN002",
            Description = "Second Transaction",
            TransactionDate = new DateTime(2025, 1, 16),
            PurchaseAmount = 250.75m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Transactions.AddRange(transaction1, transaction2);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByCustomIdAsync("TXN002");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("TXN002", result.CustomId);
        Assert.AreEqual("Second Transaction", result.Description);
        Assert.AreNotEqual("TXN001", result.CustomId);
    }

    [TestMethod]
    public async Task CreateAsync_WhenValidTransaction_CreatesAndReturnsTransaction()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new TransactionRepository(context);

        var transaction = new Transaction
        {
            CustomId = "TXN001",
            Description = "New Transaction",
            TransactionDate = new DateTime(2025, 1, 15),
            PurchaseAmount = 100.50m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = await repository.CreateAsync(transaction);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("TXN001", result.CustomId);
        Assert.AreEqual("New Transaction", result.Description);
        Assert.IsTrue(result.Id > 0); // Should have been assigned an ID

        // Verify it was actually saved to the database
        var savedTransaction = await context.Transactions.FindAsync(result.Id);
        Assert.IsNotNull(savedTransaction);
        Assert.AreEqual("TXN001", savedTransaction.CustomId);
    }

    [TestMethod]
    public async Task CreateAsync_WhenTransactionCreated_SetsCreatedAndUpdatedTimes()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new TransactionRepository(context);

        var beforeCreate = DateTime.UtcNow;
        var transaction = new Transaction
        {
            CustomId = "TXN001",
            Description = "New Transaction",
            TransactionDate = new DateTime(2025, 1, 15),
            PurchaseAmount = 100.50m
        };

        // Act
        var result = await repository.CreateAsync(transaction);
        var afterCreate = DateTime.UtcNow;

        // Assert
        Assert.IsTrue(result.CreatedAt >= beforeCreate && result.CreatedAt <= afterCreate);
        Assert.IsTrue(result.UpdatedAt >= beforeCreate && result.UpdatedAt <= afterCreate);
        // CreatedAt and UpdatedAt should be very close (within 1 second)
        var timeDifference = Math.Abs((result.CreatedAt - result.UpdatedAt).TotalSeconds);
        Assert.IsTrue(timeDifference < 1, $"CreatedAt and UpdatedAt should be nearly identical, but differ by {timeDifference} seconds");
    }

    [TestMethod]
    public async Task UpdateAsync_WhenTransactionExists_UpdatesAndReturnsTransaction()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new TransactionRepository(context);

        var originalTransaction = new Transaction
        {
            CustomId = "TXN001",
            Description = "Original Description",
            TransactionDate = new DateTime(2025, 1, 15),
            PurchaseAmount = 100.50m,
            CreatedAt = DateTime.UtcNow.AddHours(-1),
            UpdatedAt = DateTime.UtcNow.AddHours(-1)
        };

        context.Transactions.Add(originalTransaction);
        await context.SaveChangesAsync();

        var updatedTransaction = new Transaction
        {
            Id = originalTransaction.Id,
            CustomId = "TXN001",
            Description = "Updated Description",
            TransactionDate = new DateTime(2025, 1, 16),
            PurchaseAmount = 200.75m,
            CreatedAt = originalTransaction.CreatedAt
        };

        var beforeUpdate = DateTime.UtcNow;

        // Act
        var result = await repository.UpdateAsync(updatedTransaction);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Updated Description", result.Description);
        Assert.AreEqual(new DateTime(2025, 1, 16), result.TransactionDate);
        Assert.AreEqual(200.75m, result.PurchaseAmount);
        Assert.AreEqual(originalTransaction.CreatedAt, result.CreatedAt);
        Assert.IsTrue(result.UpdatedAt >= beforeUpdate && result.UpdatedAt <= afterUpdate);
    }

    [TestMethod]
    public async Task DeleteAsync_WhenTransactionExists_DeletesAndReturnsTrue()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new TransactionRepository(context);

        var transaction = new Transaction
        {
            CustomId = "TXN001",
            Description = "To Be Deleted",
            TransactionDate = new DateTime(2025, 1, 15),
            PurchaseAmount = 100.50m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.DeleteAsync(transaction.Id);

        // Assert
        Assert.IsTrue(result);

        // Verify it was actually deleted
        var deletedTransaction = await context.Transactions.FindAsync(transaction.Id);
        Assert.IsNull(deletedTransaction);
    }

    [TestMethod]
    public async Task DeleteAsync_WhenTransactionDoesNotExist_ReturnsFalse()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new TransactionRepository(context);

        // Act
        var result = await repository.DeleteAsync(999);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task ExistsAsync_WhenTransactionExists_ReturnsTrue()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new TransactionRepository(context);

        var transaction = new Transaction
        {
            CustomId = "TXN001",
            Description = "Test Transaction",
            TransactionDate = new DateTime(2025, 1, 15),
            PurchaseAmount = 100.50m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync("TXN001");

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task ExistsAsync_WhenTransactionDoesNotExist_ReturnsFalse()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new TransactionRepository(context);

        // Act
        var result = await repository.ExistsAsync("NONEXISTENT");

        // Assert
        Assert.IsFalse(result);
    }
}
