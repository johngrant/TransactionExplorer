using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using Data.Repositories;
using System.Net;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Transactions")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(ITransactionRepository transactionRepository, ILogger<TransactionsController> logger)
    {
        _transactionRepository = transactionRepository;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all transactions
    /// </summary>
    /// <returns>List of all transactions</returns>
    /// <response code="200">Returns the list of transactions</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Transaction>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetAllAsync()
    {
        _logger.LogInformation($"Executing {nameof(GetAllAsync)}()");

        try
        {
            var dataTransactions = await _transactionRepository.GetAllAsync();
            var webApiTransactions = dataTransactions.Select(MapToWebApiModel);
            return Ok(webApiTransactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving all transactions");
            throw;
        }
        finally
        {
            _logger.LogInformation($"Executed {nameof(GetAllAsync)}()");
        }
    }

    /// <summary>
    /// Get all transactions with pagination
    /// </summary>
    /// <param name="paginationParameters">Pagination parameters</param>
    /// <returns>Paged list of transactions</returns>
    /// <response code="200">Returns the paged list of transactions</response>
    /// <response code="400">If pagination parameters are invalid</response>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<Transaction>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<PagedResponse<Transaction>>> GetAllAsync([FromQuery] PaginationParameters paginationParameters)
    {
        _logger.LogInformation($"Executing {nameof(GetAllAsync)}() with pagination");

        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving paged transactions");
            throw;
        }
        finally
        {
            _logger.LogInformation($"Executed {nameof(GetAllAsync)}() with pagination");
        }
    }

    /// <summary>
    /// Get a specific transaction by ID
    /// </summary>
    /// <param name="id">The transaction ID</param>
    /// <returns>The transaction if found</returns>
    /// <response code="200">Returns the requested transaction</response>
    /// <response code="404">If the transaction is not found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Transaction), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<Transaction>> GetAsync(int id)
    {
        _logger.LogInformation($"Executing {nameof(GetAsync)}()");

        try
        {
            var dataTransaction = await _transactionRepository.GetByIdAsync(id);

            if (dataTransaction == null)
            {
                return NotFound($"Transaction with ID {id} not found");
            }

            var webApiTransaction = MapToWebApiModel(dataTransaction);
            return Ok(webApiTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving transaction by ID");
            throw;
        }
        finally
        {
            _logger.LogInformation($"Executed {nameof(GetAsync)}()");
        }
    }

    /// <summary>
    /// Create a new transaction
    /// </summary>
    /// <param name="request">The transaction creation request</param>
    /// <returns>The created transaction</returns>
    /// <response code="201">Returns the newly created transaction</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(Transaction), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<Transaction>> CreateAsync([FromBody] CreateTransactionRequest request)
    {
        _logger.LogInformation($"Executing {nameof(CreateAsync)}()");

        try
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
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            var createdTransaction = await _transactionRepository.CreateAsync(dataTransaction);
            var webApiTransaction = MapToWebApiModel(createdTransaction);

            return Created($"/api/transactions/{webApiTransaction.Id}", webApiTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating transaction");
            throw;
        }
        finally
        {
            _logger.LogInformation($"Executed {nameof(CreateAsync)}()");
        }
    }

    /// <summary>
    /// Delete a transaction by ID
    /// </summary>
    /// <param name="id">The transaction ID</param>
    /// <returns>No content if successful, NotFound if transaction doesn't exist</returns>
    /// <response code="204">If the transaction was successfully deleted</response>
    /// <response code="404">If the transaction is not found</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        _logger.LogInformation($"Executing {nameof(DeleteAsync)}()");

        try
        {
            var deleted = await _transactionRepository.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound($"Transaction with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting transaction");
            throw;
        }
        finally
        {
            _logger.LogInformation($"Executed {nameof(DeleteAsync)}()");
        }
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
