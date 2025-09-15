using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

/// <summary>
/// Pagination parameters for API requests
/// </summary>
public class PaginationParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    /// <summary>
    /// The page number (1-based). Defaults to 1.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// The number of items per page. Defaults to 10, maximum is 100.
    /// </summary>
    [Range(1, MaxPageSize, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}
