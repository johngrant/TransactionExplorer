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
    /// Get all transactions with pagination
    /// </summary>
    /// <param name="paginationParameters">Pagination parameters</param>
    /// <returns>Paged list of transactions</returns>
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResponse<Transaction>>> GetAllAsync([FromQuery] PaginationParameters paginationParameters)
    {
        // Calculate skip value (0-based indexing)
        var skip = (paginationParameters.PageNumber - 1) * paginationParameters.PageSize;

        // Get transactions with pagination using repository's general purpose Get method
        var dataTransactions = await _transactionRepository.GetAsync(
            predicate: null, // No filtering, get all transactions
            orderBy: query => query.OrderByDescending(t => t.TransactionDate), // Order by transaction date descending
            skip: skip,
            take: paginationParameters.PageSize);

        // Get total count for pagination metadata
        var totalItems = await _transactionRepository.CountAsync();

        // Map to web API models
        var webApiTransactions = dataTransactions.Select(MapToWebApiModel);

        // Create paged response
        var pagedResponse = new PagedResponse<Transaction>(
            webApiTransactions,
            paginationParameters.PageNumber,
            paginationParameters.PageSize,
            totalItems);

        return Ok(pagedResponse);
    }

    /// <summary>
    /// Get a specific transaction by ID
    /// </summary>
    /// <param name="id">The transaction ID</param>
    /// <returns>The transaction if found</returns>
    [HttpGet("{id:int}")]
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

        return Created($"/api/transactions/{webApiTransaction.Id}", webApiTransaction);
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
