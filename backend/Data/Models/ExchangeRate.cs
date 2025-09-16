using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

public class ExchangeRate
{
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateOnly RecordDate { get; set; }

    [Required]
    [StringLength(100)]
    public string CountryCurrencyDesc { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(19,6)")]
    public decimal ExchangeRateValue { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateOnly EffectiveDate { get; set; }

    [Column(TypeName = "datetimeoffset")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
