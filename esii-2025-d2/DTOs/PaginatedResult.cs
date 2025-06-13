using System.Collections.Generic;

namespace esii_2025_d2.DTOs;

public class PaginatedResult<T>
{
    /// <summary>
    /// The current page number (1-based)
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// The items for the current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Additional metadata about the search/filter operation
    /// </summary>
    public object? Metadata { get; set; }

    public PaginatedResult()
    {
    }

    public PaginatedResult(IEnumerable<T> items, int totalItems, int page, int pageSize)
    {
        Items = items;
        TotalItems = totalItems;
        Page = page;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    }
}
