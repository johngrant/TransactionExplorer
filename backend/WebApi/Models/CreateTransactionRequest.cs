using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class CreateTransactionRequest
{
    [Required]
    [StringLength(100, ErrorMessage = "CustomId cannot exceed 100 characters")]
    public string CustomId { get; set; } = string.Empty;

    [Required]
    [StringLength(50, ErrorMessage = "Description cannot exceed 50 characters")]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime TransactionDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(19,2)")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Purchase amount must be greater than 0")]
    public decimal PurchaseAmount { get; set; }
}
