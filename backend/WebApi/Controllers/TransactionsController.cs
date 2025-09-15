using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using Data.Repositories;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionsController(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    /// <summary>
    /// Get all transactions
    /// </summary>
    /// <returns>List of all transactions</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetAllAsync()
    {
        var dataTransactions = await _transactionRepository.GetAllAsync();
        var webApiTransactions = dataTransactions.Select(MapToWebApiModel);
        return Ok(webApiTransactions);
    }

    /// <summary>
    /// Get a specific transaction by ID
    /// </summary>
    /// <param name="id">The transaction ID</param>
    /// <returns>The transaction if found</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Transaction>> GetAsync(int id)
    {
        var dataTransaction = await _transactionRepository.GetByIdAsync(id);

        if (dataTransaction == null)
        {
            return NotFound($"Transaction with ID {id} not found");
        }

        var webApiTransaction = MapToWebApiModel(dataTransaction);
        return Ok(webApiTransaction);
    }

    /// <summary>
    /// Create a new transaction
    /// </summary>
    /// <param name="request">The transaction creation request</param>
    /// <returns>The created transaction</returns>
    [HttpPost]
    public async Task<ActionResult<Transaction>> CreateAsync([FromBody] CreateTransactionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var dataTransaction = new Data.Models.Transaction
        {
            CustomId = request.CustomId,
            Description = request.Description,
            TransactionDate = request.TransactionDate,
            PurchaseAmount = request.PurchaseAmount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdTransaction = await _transactionRepository.CreateAsync(dataTransaction);
        var webApiTransaction = MapToWebApiModel(createdTransaction);

        return CreatedAtAction(nameof(GetAsync), new { id = webApiTransaction.Id }, webApiTransaction);
    }

    /// <summary>
    /// Maps a Data.Models.Transaction to a WebApi.Models.Transaction
    /// </summary>
    /// <param name="dataTransaction">The data model transaction</param>
    /// <returns>The web API model transaction</returns>
    private static Transaction MapToWebApiModel(Data.Models.Transaction dataTransaction)
    {
        return new Transaction
        {
            Id = dataTransaction.Id,
            CustomId = dataTransaction.CustomId,
            Description = dataTransaction.Description,
            TransactionDate = dataTransaction.TransactionDate,
            PurchaseAmount = dataTransaction.PurchaseAmount,
            CreatedAt = dataTransaction.CreatedAt,
            UpdatedAt = dataTransaction.UpdatedAt
        };
    }
}
