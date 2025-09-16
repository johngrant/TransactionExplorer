using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using WebApi.Models;
using Data.Repositories;

namespace Tests;

/// <summary>
/// Unit tests for the TransactionsController.
///
/// These tests verify that the controller properly uses the injected repository dependencies
/// and handles various scenarios correctly.
/// </summary>
[TestClass]
public class TransactionsControllerTests
{
    private TransactionsController _controller = null!;
    private Mock<ITransactionRepository> _mockTransactionRepository = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockTransactionRepository = new Mock<ITransactionRepository>();

        _controller = new TransactionsController(
            _mockTransactionRepository.Object);
    }

    [TestMethod]
    public async Task GetAll_ReturnsEmptyList_WhenNoTransactions()
    {
        // Arrange
        _mockTransactionRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new List<Data.Models.Transaction>());

        // Act
        var result = await _controller.GetAllAsync();

        // Assert
        _mockTransactionRepository.Verify(repo => repo.GetAllAsync(), Times.Once);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var transactions = okResult.Value as IEnumerable<Transaction>;
        Assert.IsNotNull(transactions);
        Assert.AreEqual(0, transactions.Count());
    }

    [TestMethod]
    public async Task Create_ReturnsCreatedTransaction_WhenValidRequest()
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            CustomId = "TEST-001",
            Description = "Test transaction",
            TransactionDate = DateOnly.FromDateTime(DateTime.Now),
            PurchaseAmount = 100.50m
        };

        var dataTransaction = new Data.Models.Transaction
        {
            Id = 1,
            CustomId = request.CustomId,
            Description = request.Description,
            TransactionDate = request.TransactionDate,
            PurchaseAmount = request.PurchaseAmount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockTransactionRepository
            .Setup(repo => repo.CreateAsync(It.IsAny<Data.Models.Transaction>()))
            .ReturnsAsync(dataTransaction);

        // Act
        var result = await _controller.CreateAsync(request);

        // Assert
        _mockTransactionRepository.Verify(repo => repo.CreateAsync(It.IsAny<Data.Models.Transaction>()), Times.Once);

        var createdResult = result.Result as CreatedResult;
        Assert.IsNotNull(createdResult);

        var transaction = createdResult.Value as Transaction;
        Assert.IsNotNull(transaction);
        Assert.AreEqual("Test transaction", transaction.Description);
        Assert.AreEqual("TEST-001", transaction.CustomId);
        Assert.AreEqual(100.50m, transaction.PurchaseAmount);
        Assert.AreEqual(1, transaction.Id);
    }

    [TestMethod]
    public async Task Get_ReturnsNotFound_WhenTransactionDoesNotExist()
    {
        // Arrange
        _mockTransactionRepository
            .Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((Data.Models.Transaction?)null);

        // Act
        var result = await _controller.GetAsync(999);

        // Assert
        _mockTransactionRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task Get_ReturnsTransaction_WhenTransactionExists()
    {
        // Arrange
        var dataTransaction = new Data.Models.Transaction
        {
            Id = 1,
            CustomId = "TEST-002",
            Description = "Test transaction for retrieval",
            TransactionDate = DateOnly.FromDateTime(DateTime.Now),
            PurchaseAmount = 75.25m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockTransactionRepository
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(dataTransaction);

        // Act
        var result = await _controller.GetAsync(1);

        // Assert
        _mockTransactionRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult, "Get should return OkObjectResult");

        var transaction = okResult.Value as Transaction;
        Assert.IsNotNull(transaction, "Retrieved transaction should not be null");
        Assert.AreEqual(1, transaction.Id);
        Assert.AreEqual("Test transaction for retrieval", transaction.Description);
        Assert.AreEqual("TEST-002", transaction.CustomId);
        Assert.AreEqual(75.25m, transaction.PurchaseAmount);
    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            CustomId = "", // Invalid - required field
            Description = "Test transaction",
            TransactionDate = DateOnly.FromDateTime(DateTime.Now),
            PurchaseAmount = 100.50m
        };

        // Manually invalidate model state to simulate validation failure
        _controller.ModelState.AddModelError("CustomId", "CustomId is required");

        // Act
        var result = await _controller.CreateAsync(request);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenPurchaseAmountIsZero()
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            CustomId = "TEST-003",
            Description = "Test transaction",
            TransactionDate = DateOnly.FromDateTime(DateTime.Now),
            PurchaseAmount = 0m // Invalid - must be greater than 0
        };

        // Manually invalidate model state to simulate validation failure
        _controller.ModelState.AddModelError("PurchaseAmount", "Purchase amount must be greater than 0");

        // Act
        var result = await _controller.CreateAsync(request);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
    }

    [TestMethod]
    public async Task GetAll_ReturnsTransactions_WhenTransactionsExist()
    {
        // Arrange
        var dataTransactions = new List<Data.Models.Transaction>
        {
            new Data.Models.Transaction
            {
                Id = 1,
                CustomId = "TEST-004",
                Description = "First transaction",
                TransactionDate = DateOnly.FromDateTime(DateTime.Now),
                PurchaseAmount = 50.00m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Data.Models.Transaction
            {
                Id = 2,
                CustomId = "TEST-005",
                Description = "Second transaction",
                TransactionDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                PurchaseAmount = 125.75m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _mockTransactionRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(dataTransactions);

        // Act
        var result = await _controller.GetAllAsync();

        // Assert
        _mockTransactionRepository.Verify(repo => repo.GetAllAsync(), Times.Once);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var transactions = okResult.Value as IEnumerable<Transaction>;
        Assert.IsNotNull(transactions);
        Assert.AreEqual(2, transactions.Count());

        var transactionList = transactions.ToList();
        Assert.IsTrue(transactionList.Any(t => t.CustomId == "TEST-004"));
        Assert.IsTrue(transactionList.Any(t => t.CustomId == "TEST-005"));
    }

    [TestMethod]
    public async Task Create_SetsCorrectTimestamps_WhenCreatingTransaction()
    {
        // Arrange
        var beforeCreate = DateTime.UtcNow;
        var request = new CreateTransactionRequest
        {
            CustomId = "TEST-006",
            Description = "Test transaction with timestamps",
            TransactionDate = DateOnly.FromDateTime(DateTime.Now),
            PurchaseAmount = 99.99m
        };

        var dataTransaction = new Data.Models.Transaction
        {
            Id = 1,
            CustomId = request.CustomId,
            Description = request.Description,
            TransactionDate = request.TransactionDate,
            PurchaseAmount = request.PurchaseAmount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockTransactionRepository
            .Setup(repo => repo.CreateAsync(It.IsAny<Data.Models.Transaction>()))
            .ReturnsAsync(dataTransaction);

        // Act
        var result = await _controller.CreateAsync(request);

        // Assert
        _mockTransactionRepository.Verify(repo => repo.CreateAsync(It.IsAny<Data.Models.Transaction>()), Times.Once);

        var createdResult = result.Result as CreatedResult;
        Assert.IsNotNull(createdResult);

        var transaction = createdResult.Value as Transaction;
        Assert.IsNotNull(transaction);
        Assert.IsTrue(transaction.CreatedAt >= beforeCreate);
        Assert.IsTrue(transaction.UpdatedAt >= beforeCreate);
    }

    [TestMethod]
    public async Task Get_ReturnsNotFound_WhenNegativeId()
    {
        // Arrange
        _mockTransactionRepository
            .Setup(repo => repo.GetByIdAsync(-1))
            .ReturnsAsync((Data.Models.Transaction?)null);

        // Act
        var result = await _controller.GetAsync(-1);

        // Assert
        _mockTransactionRepository.Verify(repo => repo.GetByIdAsync(-1), Times.Once);
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task CreateAsync_ReturnsCreated_WithCorrectLocationUrl()
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            CustomId = "TEST-007",
            Description = "Test route values",
            TransactionDate = DateOnly.FromDateTime(DateTime.Now),
            PurchaseAmount = 150.00m
        };

        var dataTransaction = new Data.Models.Transaction
        {
            Id = 1,
            CustomId = request.CustomId,
            Description = request.Description,
            TransactionDate = request.TransactionDate,
            PurchaseAmount = request.PurchaseAmount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockTransactionRepository
            .Setup(repo => repo.CreateAsync(It.IsAny<Data.Models.Transaction>()))
            .ReturnsAsync(dataTransaction);

        // Act
        var result = await _controller.CreateAsync(request);

        // Assert
        _mockTransactionRepository.Verify(repo => repo.CreateAsync(It.IsAny<Data.Models.Transaction>()), Times.Once);

        var createdResult = result.Result as CreatedResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual("/api/transactions/1", createdResult.Location);

        var transaction = createdResult.Value as Transaction;
        Assert.IsNotNull(transaction);
        Assert.AreEqual(1, transaction.Id);
    }

    #region Paged GetAll Tests

    [TestMethod]
    public async Task GetAllPaged_ReturnsFirstPage_WithValidPagination()
    {
        // Arrange
        var paginationParams = new WebApi.Models.PaginationParameters { PageNumber = 1, PageSize = 2 };
        var dataTransactions = new List<Data.Models.Transaction>
        {
            new() { Id = 1, CustomId = "T1", Description = "Transaction 1", TransactionDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)), PurchaseAmount = 100m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 2, CustomId = "T2", Description = "Transaction 2", TransactionDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), PurchaseAmount = 200m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        _mockTransactionRepository
            .Setup(repo => repo.GetAsync(
                null, // no predicate
                It.IsAny<Func<IQueryable<Data.Models.Transaction>, IOrderedQueryable<Data.Models.Transaction>>>(),
                0, // skip
                2)) // take
            .ReturnsAsync(dataTransactions);

        _mockTransactionRepository
            .Setup(repo => repo.CountAsync(null))
            .ReturnsAsync(5); // total of 5 transactions

        // Act
        var result = await _controller.GetAllAsync(paginationParams);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var pagedResponse = okResult.Value as WebApi.Models.PagedResponse<Transaction>;
        Assert.IsNotNull(pagedResponse);
        Assert.AreEqual(1, pagedResponse.PageNumber);
        Assert.AreEqual(2, pagedResponse.PageSize);
        Assert.AreEqual(5, pagedResponse.TotalItems);
        Assert.AreEqual(3, pagedResponse.TotalPages);
        Assert.IsFalse(pagedResponse.HasPreviousPage);
        Assert.IsTrue(pagedResponse.HasNextPage);
        Assert.AreEqual(2, pagedResponse.Items.Count());
    }

    [TestMethod]
    public async Task GetAllPaged_ReturnsSecondPage_WithValidPagination()
    {
        // Arrange
        var paginationParams = new WebApi.Models.PaginationParameters { PageNumber = 2, PageSize = 2 };
        var dataTransactions = new List<Data.Models.Transaction>
        {
            new() { Id = 3, CustomId = "T3", Description = "Transaction 3", TransactionDate = DateOnly.FromDateTime(DateTime.Now), PurchaseAmount = 300m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 4, CustomId = "T4", Description = "Transaction 4", TransactionDate = DateOnly.FromDateTime(DateTime.Now), PurchaseAmount = 400m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        _mockTransactionRepository
            .Setup(repo => repo.GetAsync(
                null, // no predicate
                It.IsAny<Func<IQueryable<Data.Models.Transaction>, IOrderedQueryable<Data.Models.Transaction>>>(),
                2, // skip
                2)) // take
            .ReturnsAsync(dataTransactions);

        _mockTransactionRepository
            .Setup(repo => repo.CountAsync(null))
            .ReturnsAsync(5); // total of 5 transactions

        // Act
        var result = await _controller.GetAllAsync(paginationParams);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var pagedResponse = okResult.Value as WebApi.Models.PagedResponse<Transaction>;
        Assert.IsNotNull(pagedResponse);
        Assert.AreEqual(2, pagedResponse.PageNumber);
        Assert.AreEqual(2, pagedResponse.PageSize);
        Assert.AreEqual(5, pagedResponse.TotalItems);
        Assert.AreEqual(3, pagedResponse.TotalPages);
        Assert.IsTrue(pagedResponse.HasPreviousPage);
        Assert.IsTrue(pagedResponse.HasNextPage);
        Assert.AreEqual(2, pagedResponse.Items.Count());
    }

    [TestMethod]
    public async Task GetAllPaged_ReturnsLastPage_WithValidPagination()
    {
        // Arrange
        var paginationParams = new WebApi.Models.PaginationParameters { PageNumber = 3, PageSize = 2 };
        var dataTransactions = new List<Data.Models.Transaction>
        {
            new() { Id = 5, CustomId = "T5", Description = "Transaction 5", TransactionDate = DateOnly.FromDateTime(DateTime.Now), PurchaseAmount = 500m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        _mockTransactionRepository
            .Setup(repo => repo.GetAsync(
                null, // no predicate
                It.IsAny<Func<IQueryable<Data.Models.Transaction>, IOrderedQueryable<Data.Models.Transaction>>>(),
                4, // skip
                2)) // take
            .ReturnsAsync(dataTransactions);

        _mockTransactionRepository
            .Setup(repo => repo.CountAsync(null))
            .ReturnsAsync(5); // total of 5 transactions

        // Act
        var result = await _controller.GetAllAsync(paginationParams);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var pagedResponse = okResult.Value as WebApi.Models.PagedResponse<Transaction>;
        Assert.IsNotNull(pagedResponse);
        Assert.AreEqual(3, pagedResponse.PageNumber);
        Assert.AreEqual(2, pagedResponse.PageSize);
        Assert.AreEqual(5, pagedResponse.TotalItems);
        Assert.AreEqual(3, pagedResponse.TotalPages);
        Assert.IsTrue(pagedResponse.HasPreviousPage);
        Assert.IsFalse(pagedResponse.HasNextPage);
        Assert.AreEqual(1, pagedResponse.Items.Count());
    }

    [TestMethod]
    public async Task GetAllPaged_ReturnsEmptyPage_WhenNoTransactions()
    {
        // Arrange
        var paginationParams = new WebApi.Models.PaginationParameters { PageNumber = 1, PageSize = 10 };

        _mockTransactionRepository
            .Setup(repo => repo.GetAsync(
                null, // no predicate
                It.IsAny<Func<IQueryable<Data.Models.Transaction>, IOrderedQueryable<Data.Models.Transaction>>>(),
                0, // skip
                10)) // take
            .ReturnsAsync(new List<Data.Models.Transaction>());

        _mockTransactionRepository
            .Setup(repo => repo.CountAsync(null))
            .ReturnsAsync(0);

        // Act
        var result = await _controller.GetAllAsync(paginationParams);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var pagedResponse = okResult.Value as WebApi.Models.PagedResponse<Transaction>;
        Assert.IsNotNull(pagedResponse);
        Assert.AreEqual(1, pagedResponse.PageNumber);
        Assert.AreEqual(10, pagedResponse.PageSize);
        Assert.AreEqual(0, pagedResponse.TotalItems);
        Assert.AreEqual(0, pagedResponse.TotalPages);
        Assert.IsFalse(pagedResponse.HasPreviousPage);
        Assert.IsFalse(pagedResponse.HasNextPage);
        Assert.AreEqual(0, pagedResponse.Items.Count());
    }

    [TestMethod]
    public async Task GetAllPaged_ShouldOrderByTransactionDateDescending()
    {
        // Arrange
        var paginationParams = new WebApi.Models.PaginationParameters { PageNumber = 1, PageSize = 10 };
        var dataTransactions = new List<Data.Models.Transaction>
        {
            new() { Id = 2, CustomId = "T2", Description = "Transaction 2", TransactionDate = DateOnly.FromDateTime(DateTime.Now), PurchaseAmount = 200m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 1, CustomId = "T1", Description = "Transaction 1", TransactionDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), PurchaseAmount = 100m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        _mockTransactionRepository
            .Setup(repo => repo.GetAsync(
                null, // no predicate
                It.IsAny<Func<IQueryable<Data.Models.Transaction>, IOrderedQueryable<Data.Models.Transaction>>>(),
                0, // skip
                10)) // take
            .ReturnsAsync(dataTransactions);

        _mockTransactionRepository
            .Setup(repo => repo.CountAsync(null))
            .ReturnsAsync(2);

        // Act
        var result = await _controller.GetAllAsync(paginationParams);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var pagedResponse = okResult.Value as WebApi.Models.PagedResponse<Transaction>;
        Assert.IsNotNull(pagedResponse);

        // Verify that the ordering function was called with TransactionDate descending
        _mockTransactionRepository.Verify(
            repo => repo.GetAsync(
                null,
                It.IsAny<Func<IQueryable<Data.Models.Transaction>, IOrderedQueryable<Data.Models.Transaction>>>(),
                0,
                10),
            Times.Once);
    }

    #endregion
}
