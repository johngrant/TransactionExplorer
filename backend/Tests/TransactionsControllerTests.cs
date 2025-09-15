using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using WebApi.Models;
using Data.Repositories;

namespace Tests;

/// <summary>
/// Unit tests for the TransactionsController.
///
/// Note: The current controller implementation uses static in-memory storage for demonstration purposes,
/// but also has injected repository dependencies for future use. These tests set up mocks for the
/// repositories but currently test against the static storage implementation.
///
/// When the controller is updated to use the injected repositories, these tests should be updated
/// to verify the repository interactions using the mocked objects.
/// </summary>
[TestClass]
[DoNotParallelize]
public class TransactionsControllerTests
{
    private TransactionsController _controller = null!;
    private Mock<ITransactionRepository> _mockTransactionRepository = null!;
    private Mock<IExchangeRateRepository> _mockExchangeRateRepository = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _mockExchangeRateRepository = new Mock<IExchangeRateRepository>();

        _controller = new TransactionsController(
            _mockTransactionRepository.Object,
            _mockExchangeRateRepository.Object);

        // Clear any existing static data before each test
        TransactionsController.ClearTransactions();
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Clear any existing static data after each test
        TransactionsController.ClearTransactions();
    }

    [TestMethod]
    public async Task GetAll_ReturnsEmptyList_WhenNoTransactions()
    {
        // Act
        var result = await _controller.GetAllAsync();

        // Assert
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
            TransactionDate = DateTime.Now,
            PurchaseAmount = 100.50m
        };

        // Act
        var result = await _controller.CreateAsync(request);

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);

        var transaction = createdResult.Value as Transaction;
        Assert.IsNotNull(transaction);
        Assert.AreEqual("Test transaction", transaction.Description);
        Assert.AreEqual("TEST-001", transaction.CustomId);
        Assert.AreEqual(100.50m, transaction.PurchaseAmount);
        Assert.IsTrue(transaction.Id > 0);
    }

    [TestMethod]
    public async Task Get_ReturnsNotFound_WhenTransactionDoesNotExist()
    {
        // Act
        var result = await _controller.GetAsync(999);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task Get_ReturnsTransaction_WhenTransactionExists()
    {
        // Arrange - Make sure we start with a clean state
        TransactionsController.ClearTransactions();

        var createRequest = new CreateTransactionRequest
        {
            CustomId = "TEST-002",
            Description = "Test transaction for retrieval",
            TransactionDate = DateTime.Now,
            PurchaseAmount = 75.25m
        };
        var createResult = await _controller.CreateAsync(createRequest);
        var createdAtResult = createResult.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdAtResult, "Create should return CreatedAtActionResult");

        var createdTransaction = createdAtResult.Value as Transaction;
        Assert.IsNotNull(createdTransaction, "Created transaction should not be null");

        // Act
        var result = await _controller.GetAsync(createdTransaction.Id);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult, "Get should return OkObjectResult");

        var transaction = okResult.Value as Transaction;
        Assert.IsNotNull(transaction, "Retrieved transaction should not be null");
        Assert.AreEqual(createdTransaction.Id, transaction.Id);
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
            TransactionDate = DateTime.Now,
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
            TransactionDate = DateTime.Now,
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
        // Arrange - Create a few transactions
        var request1 = new CreateTransactionRequest
        {
            CustomId = "TEST-004",
            Description = "First transaction",
            TransactionDate = DateTime.Now,
            PurchaseAmount = 50.00m
        };

        var request2 = new CreateTransactionRequest
        {
            CustomId = "TEST-005",
            Description = "Second transaction",
            TransactionDate = DateTime.Now.AddDays(-1),
            PurchaseAmount = 125.75m
        };

        await _controller.CreateAsync(request1);
        await _controller.CreateAsync(request2);

        // Act
        var result = await _controller.GetAllAsync();

        // Assert
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
            TransactionDate = DateTime.Now.Date,
            PurchaseAmount = 99.99m
        };

        // Act
        var result = await _controller.CreateAsync(request);

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);

        var transaction = createdResult.Value as Transaction;
        Assert.IsNotNull(transaction);
        Assert.IsTrue(transaction.CreatedAt >= beforeCreate);
        Assert.IsTrue(transaction.UpdatedAt >= beforeCreate);
        Assert.AreEqual(transaction.CreatedAt, transaction.UpdatedAt); // Should be same on creation
    }

    [TestMethod]
    public async Task Get_ReturnsNotFound_WhenNegativeId()
    {
        // Act
        var result = await _controller.GetAsync(-1);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task CreateAsync_ReturnsCreatedAtAction_WithCorrectRouteValues()
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            CustomId = "TEST-007",
            Description = "Test route values",
            TransactionDate = DateTime.Now,
            PurchaseAmount = 150.00m
        };

        // Act
        var result = await _controller.CreateAsync(request);

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(nameof(TransactionsController.GetAsync), createdResult.ActionName);

        var transaction = createdResult.Value as Transaction;
        Assert.IsNotNull(transaction);

        var routeValues = createdResult.RouteValues;
        Assert.IsNotNull(routeValues);
        Assert.AreEqual(transaction.Id, routeValues["id"]);
    }

    // Example test showing how to test with repository mocks when controller is updated
    // This test is currently commented out because the controller doesn't use repositories yet
    /*
    [TestMethod]
    public async Task GetAllAsync_CallsRepository_WhenUsingRepositoryPattern()
    {
        // Arrange
        var expectedTransactions = new List<Data.Models.Transaction>
        {
            new Data.Models.Transaction { Id = 1, CustomId = "TEST-001", Description = "Test 1", PurchaseAmount = 100m },
            new Data.Models.Transaction { Id = 2, CustomId = "TEST-002", Description = "Test 2", PurchaseAmount = 200m }
        };

        _mockTransactionRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedTransactions);

        // Act
        var result = await _controller.GetAllAsync();

        // Assert
        _mockTransactionRepository.Verify(repo => repo.GetAllAsync(), Times.Once);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var transactions = okResult.Value as IEnumerable<Data.Models.Transaction>;
        Assert.IsNotNull(transactions);
        Assert.AreEqual(2, transactions.Count());
    }
    */
}
