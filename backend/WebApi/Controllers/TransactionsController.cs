using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    // Lazy-initialized in-memory storage for demonstration purposes
    private static readonly Lazy<List<Transaction>> _lazyTransactions = new(() => new List<Transaction>());
    private static int _nextId = 1;

    private static List<Transaction> _transactions => _lazyTransactions.Value;

    /// <summary>
    /// Clear all transactions (for testing purposes)
    /// </summary>
    public static void ClearTransactions()
    {
        _transactions.Clear();
        _nextId = 1;
    }

    /// <summary>
    /// Get all transactions
    /// </summary>
    /// <returns>List of all transactions</returns>
    [HttpGet]
    public ActionResult<IEnumerable<Transaction>> GetAll()
    {
        return Ok(_transactions);
    }

    /// <summary>
    /// Get a specific transaction by ID
    /// </summary>
    /// <param name="id">The transaction ID</param>
    /// <returns>The transaction if found</returns>
    [HttpGet("{id}")]
    public ActionResult<Transaction> Get(int id)
    {
        var transaction = _transactions.FirstOrDefault(t => t.Id == id);

        if (transaction == null)
        {
            return NotFound($"Transaction with ID {id} not found");
        }

        return Ok(transaction);
    }

    /// <summary>
    /// Create a new transaction
    /// </summary>
    /// <param name="request">The transaction creation request</param>
    /// <returns>The created transaction</returns>
    [HttpPost]
    public ActionResult<Transaction> Create([FromBody] CreateTransactionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var transaction = new Transaction
        {
            Id = _nextId++,
            Description = request.Description
        };

        _transactions.Add(transaction);

        return CreatedAtAction(nameof(Get), new { id = transaction.Id }, transaction);
    }
}
