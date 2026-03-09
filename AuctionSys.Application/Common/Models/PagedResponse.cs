namespace AuctionSys.Application.Common.Models;

public class PagedResponse<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    
    // For Cursor Pagination
    public string? NextCursor { get; set; }
    
    // For Offset Pagination
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => !string.IsNullOrEmpty(NextCursor) || PageNumber < TotalPages;
}
