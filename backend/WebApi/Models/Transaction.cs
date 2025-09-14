using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Transaction
{
    public int Id { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "Description cannot exceed 50 characters")]
    public string Description { get; set; } = string.Empty;
}
