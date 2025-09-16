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
    public DateOnly TransactionDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(19,2)")]
    public decimal PurchaseAmount { get; set; }

    [Column(TypeName = "datetimeoffset")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [Column(TypeName = "datetimeoffset")]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public void MarkAsUpdated()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
