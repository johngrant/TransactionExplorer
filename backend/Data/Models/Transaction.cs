using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

public class Transaction
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string CustomId { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "date")]
    public DateTime TransactionDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(19,2)")]
    public decimal PurchaseAmount { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime UpdatedAt { get; set; }
}
