using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class CreateTransactionRequest
{
    [Required]
    [StringLength(50, ErrorMessage = "Description cannot exceed 50 characters")]
    public string Description { get; set; } = string.Empty;
}
