using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;
using WebApi.Models;

namespace Tests;

[TestClass]
[DoNotParallelize]
public class TransactionsControllerTests
{
    private TransactionsController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _controller = new TransactionsController();
        // Clear any existing data before each test
        TransactionsController.ClearTransactions();
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Clear any existing data after each test
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
            Description = "Test transaction"
        };

        // Act
        var result = await _controller.CreateAsync(request);

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);

        var transaction = createdResult.Value as Transaction;
        Assert.IsNotNull(transaction);
        Assert.AreEqual("Test transaction", transaction.Description);
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
            Description = "Test transaction for retrieval"
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
    }
}
