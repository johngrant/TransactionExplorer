using Data.Context;
using Data.Models;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests;

[TestClass]
public class ExchangeRateRepositoryTests
{
    private TransactionExplorerDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<TransactionExplorerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new TransactionExplorerDbContext(options);
        return context;
    }

    #region GetAllAsync Tests

    [TestMethod]
    public async Task GetAllAsync_WhenNoExchangeRates_ReturnsEmptyCollection()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new ExchangeRateRepository(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public async Task GetAllAsync_WhenExchangeRatesExist_ReturnsAllExchangeRates()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new ExchangeRateRepository(context);

        var exchangeRate1 = new ExchangeRate
        {
            RecordDate = new DateOnly(2025, 1, 15),
            CountryCurrencyDesc = "Canada-Dollar",
            ExchangeRateValue = 1.25m,
            EffectiveDate = new DateOnly(2025, 1, 15),
            CreatedAt = DateTime.UtcNow
        };

        var exchangeRate2 = new ExchangeRate
        {
            RecordDate = new DateOnly(2025, 1, 16),
            CountryCurrencyDesc = "Euro Zone-Euro",
            ExchangeRateValue = 0.85m,
            EffectiveDate = new DateOnly(2025, 1, 16),
            CreatedAt = DateTime.UtcNow
        };

        context.ExchangeRates.AddRange(exchangeRate1, exchangeRate2);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());

        var exchangeRates = result.ToList();
        Assert.IsTrue(exchangeRates.Any(r => r.CountryCurrencyDesc == "Canada-Dollar"));
        Assert.IsTrue(exchangeRates.Any(r => r.CountryCurrencyDesc == "Euro Zone-Euro"));
        Assert.IsTrue(exchangeRates.Any(r => r.ExchangeRateValue == 1.25m));
        Assert.IsTrue(exchangeRates.Any(r => r.ExchangeRateValue == 0.85m));
    }

    #endregion

    #region GetByIdAsync Tests

    [TestMethod]
    public async Task GetByIdAsync_WhenExchangeRateExists_ReturnsExchangeRate()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new ExchangeRateRepository(context);

        var exchangeRate = new ExchangeRate
        {
            RecordDate = new DateOnly(2025, 1, 15),
            CountryCurrencyDesc = "Canada-Dollar",
            ExchangeRateValue = 1.25m,
            EffectiveDate = new DateOnly(2025, 1, 15),
            CreatedAt = DateTime.UtcNow
        };

        context.ExchangeRates.Add(exchangeRate);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(exchangeRate.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(exchangeRate.Id, result.Id);
        Assert.AreEqual("Canada-Dollar", result.CountryCurrencyDesc);
        Assert.AreEqual(1.25m, result.ExchangeRateValue);
        Assert.AreEqual(new DateOnly(2025, 1, 15), result.EffectiveDate);
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenExchangeRateDoesNotExist_ReturnsNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new ExchangeRateRepository(context);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region GetLatestRateAsync Tests

    [TestMethod]
    public async Task GetLatestRateAsync_WhenRateExists_ReturnsLatestRateForCurrency()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new ExchangeRateRepository(context);

        var transactionDate = new DateOnly(2025, 1, 20);
        var olderRate = new ExchangeRate
        {
            RecordDate = new DateOnly(2025, 1, 10),
            CountryCurrencyDesc = "Canada-Dollar",
            ExchangeRateValue = 1.20m,
            EffectiveDate = new DateOnly(2025, 1, 10),
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };

        var newerRate = new ExchangeRate
        {
            RecordDate = new DateOnly(2025, 1, 15),
            CountryCurrencyDesc = "Canada-Dollar",
            ExchangeRateValue = 1.25m,
            EffectiveDate = new DateOnly(2025, 1, 15),
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        };

        // Future rate should be ignored
        var futureRate = new ExchangeRate
        {
            RecordDate = new DateOnly(2025, 1, 25),
            CountryCurrencyDesc = "Canada-Dollar",
            ExchangeRateValue = 1.30m,
            EffectiveDate = new DateOnly(2025, 1, 25),
            CreatedAt = DateTime.UtcNow
        };

        context.ExchangeRates.AddRange(olderRate, newerRate, futureRate);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetLatestRateAsync("Canada-Dollar", transactionDate);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1.25m, result.ExchangeRateValue);
        Assert.AreEqual(new DateOnly(2025, 1, 15), result.EffectiveDate);
    }

    [TestMethod]
    public async Task GetLatestRateAsync_WhenNoRateExistsForCurrency_ReturnsNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new ExchangeRateRepository(context);

        var transactionDate = new DateOnly(2025, 1, 20);

        // Add a rate for a different currency
        var exchangeRate = new ExchangeRate
        {
            RecordDate = new DateOnly(2025, 1, 15),
            CountryCurrencyDesc = "Euro Zone-Euro",
            ExchangeRateValue = 0.85m,
            EffectiveDate = new DateOnly(2025, 1, 15),
            CreatedAt = DateTime.UtcNow
        };

        context.ExchangeRates.Add(exchangeRate);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetLatestRateAsync("Canada-Dollar", transactionDate);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetLatestRateAsync_WhenOnlyFutureRatesExist_ReturnsNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new ExchangeRateRepository(context);

        var transactionDate = new DateOnly(2025, 1, 10);

        var futureRate = new ExchangeRate
        {
            RecordDate = new DateOnly(2025, 1, 15),
            CountryCurrencyDesc = "Canada-Dollar",
            ExchangeRateValue = 1.25m,
            EffectiveDate = new DateOnly(2025, 1, 15),
            CreatedAt = DateTime.UtcNow
        };

        context.ExchangeRates.Add(futureRate);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetLatestRateAsync("Canada-Dollar", transactionDate);

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region GetRatesForDateRangeAsync Tests

    [TestMethod]
    public async Task GetRatesForDateRangeAsync_WhenRatesExistInRange_ReturnsFilteredRates()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new ExchangeRateRepository(context);

        var startDate = new DateOnly(2025, 1, 10);
        var endDate = new DateOnly(2025, 1, 20);

        var rateBeforeRange = new ExchangeRate
        {
            RecordDate = new DateOnly(2025, 1, 5),
            CountryCurrencyDesc = "Canada-Dollar",
            ExchangeRateValue = 1.20m,
            EffectiveDate = new DateOnly(2025, 1, 5),
            CreatedAt = DateTime.UtcNow
        };

        var rateInRange1 = new ExchangeRate
        {
            RecordDate = new DateOnly(2025, 1, 15),
            CountryCurrencyDesc = "Canada-Dollar",
            ExchangeRateValue = 1.25m,
            EffectiveDate = new DateOnly(2025, 1, 15),
            CreatedAt = DateTime.UtcNow
        };

        var rateInRange2 = new ExchangeRate
        {
            RecordDate = new DateOnly(2025, 1, 18),
            CountryCurrencyDesc = "Canada-Dollar",
            ExchangeRateValue = 1.27m,
            EffectiveDate = new DateOnly(2025, 1, 18),
            CreatedAt = DateTime.UtcNow
        };

        var rateAfterRange = new ExchangeRate
        {
            RecordDate = new DateOnly(2025, 1, 25),
            CountryCurrencyDesc = "Canada-Dollar",
            ExchangeRateValue = 1.30m,
            EffectiveDate = new DateOnly(2025, 1, 25),
            CreatedAt = DateTime.UtcNow
        };

        // Different currency in range - should be excluded
        var differentCurrencyInRange = new ExchangeRate
        {
            RecordDate = new DateOnly(2025, 1, 16),
            CountryCurrencyDesc = "Euro Zone-Euro",
            ExchangeRateValue = 0.85m,
            EffectiveDate = new DateOnly(2025, 1, 16),
            CreatedAt = DateTime.UtcNow
        };

        context.ExchangeRates.AddRange(rateBeforeRange, rateInRange1, rateInRange2, rateAfterRange, differentCurrencyInRange);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetRatesForDateRangeAsync("Canada-Dollar", startDate, endDate);

        // Assert
        Assert.IsNotNull(result);
        var rates = result.ToList();
        Assert.AreEqual(2, rates.Count);
        Assert.IsTrue(rates.Any(r => r.ExchangeRateValue == 1.25m));
        Assert.IsTrue(rates.Any(r => r.ExchangeRateValue == 1.27m));
        Assert.IsTrue(rates.All(r => r.CountryCurrencyDesc == "Canada-Dollar"));
        Assert.IsTrue(rates.All(r => r.EffectiveDate >= startDate && r.EffectiveDate <= endDate));
    }

    [TestMethod]
    public async Task GetRatesForDateRangeAsync_WhenNoRatesInRange_ReturnsEmptyCollection()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new ExchangeRateRepository(context);

        var startDate = new DateOnly(2025, 1, 10);
        var endDate = new DateOnly(2025, 1, 20);

        var rateOutsideRange = new ExchangeRate
        {
            RecordDate = new DateOnly(2025, 1, 5),
            CountryCurrencyDesc = "Canada-Dollar",
            ExchangeRateValue = 1.20m,
            EffectiveDate = new DateOnly(2025, 1, 5),
            CreatedAt = DateTime.UtcNow
        };

        context.ExchangeRates.Add(rateOutsideRange);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetRatesForDateRangeAsync("Canada-Dollar", startDate, endDate);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    #endregion

    #region CreateAsync Tests

    [TestMethod]
    public async Task CreateAsync_WhenValidExchangeRate_CreatesAndReturnsExchangeRate()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new ExchangeRateRepository(context);

        var exchangeRate = new ExchangeRate
        {
            RecordDate = new DateOnly(2025, 1, 15),
            CountryCurrencyDesc = "Canada-Dollar",
            ExchangeRateValue = 1.25m,
            EffectiveDate = new DateOnly(2025, 1, 15),
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await repository.CreateAsync(exchangeRate);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Id > 0);
        Assert.AreEqual("Canada-Dollar", result.CountryCurrencyDesc);
        Assert.AreEqual(1.25m, result.ExchangeRateValue);
        Assert.AreEqual(new DateOnly(2025, 1, 15), result.EffectiveDate);

        // Verify it was saved to database
        var savedRate = await context.ExchangeRates.FindAsync(result.Id);
        Assert.IsNotNull(savedRate);
        Assert.AreEqual(result.Id, savedRate.Id);
    }

    #endregion

    #region CreateBulkAsync Tests

    [TestMethod]
    public async Task CreateBulkAsync_WhenValidExchangeRates_CreatesAndReturnsAllExchangeRates()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new ExchangeRateRepository(context);

        var exchangeRates = new[]
        {
            new ExchangeRate
            {
                RecordDate = new DateOnly(2025, 1, 15),
                CountryCurrencyDesc = "Canada-Dollar",
                ExchangeRateValue = 1.25m,
                EffectiveDate = new DateOnly(2025, 1, 15),
                CreatedAt = DateTime.UtcNow
            },
            new ExchangeRate
            {
                RecordDate = new DateOnly(2025, 1, 15),
                CountryCurrencyDesc = "Euro Zone-Euro",
                ExchangeRateValue = 0.85m,
                EffectiveDate = new DateOnly(2025, 1, 15),
                CreatedAt = DateTime.UtcNow
            },
            new ExchangeRate
            {
                RecordDate = new DateOnly(2025, 1, 15),
                CountryCurrencyDesc = "United Kingdom-Pound",
                ExchangeRateValue = 0.75m,
                EffectiveDate = new DateOnly(2025, 1, 15),
                CreatedAt = DateTime.UtcNow
            }
        };

        // Act
        var result = await repository.CreateBulkAsync(exchangeRates);

        // Assert
        Assert.IsNotNull(result);
        var resultList = result.ToList();
        Assert.AreEqual(3, resultList.Count);
        Assert.IsTrue(resultList.All(r => r.Id > 0));

        // Verify all were saved to database
        var savedRates = await context.ExchangeRates.ToListAsync();
        Assert.AreEqual(3, savedRates.Count);
        Assert.IsTrue(savedRates.Any(r => r.CountryCurrencyDesc == "Canada-Dollar"));
        Assert.IsTrue(savedRates.Any(r => r.CountryCurrencyDesc == "Euro Zone-Euro"));
        Assert.IsTrue(savedRates.Any(r => r.CountryCurrencyDesc == "United Kingdom-Pound"));
    }

    [TestMethod]
    public async Task CreateBulkAsync_WhenEmptyCollection_ReturnsEmptyCollection()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new ExchangeRateRepository(context);

        var exchangeRates = new ExchangeRate[0];

        // Act
        var result = await repository.CreateBulkAsync(exchangeRates);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());

        // Verify no records were added to database
        var savedRates = await context.ExchangeRates.ToListAsync();
        Assert.AreEqual(0, savedRates.Count);
    }

    #endregion
}
