namespace WebApi.Models;

/// <summary>
/// Generic paged response model that wraps collections with pagination metadata
/// </summary>
/// <typeparam name="T">The type of items in the collection</typeparam>
public class PagedResponse<T>
{
    /// <summary>
    /// The current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// The number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The total number of items across all pages
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// The total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage { get; set; }

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// The items in the current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Creates a new paged response
    /// </summary>
    /// <param name="items">The items for the current page</param>
    /// <param name="pageNumber">The current page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="totalItems">The total number of items across all pages</param>
    public PagedResponse(IEnumerable<T> items, int pageNumber, int pageSize, int totalItems)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalItems = totalItems;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        HasPreviousPage = pageNumber > 1;
        HasNextPage = pageNumber < TotalPages;
    }

    /// <summary>
    /// Creates an empty paged response
    /// </summary>
    public PagedResponse()
    {
    }
}
