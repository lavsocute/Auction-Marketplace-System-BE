using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Enums;
using System.Linq.Expressions;

namespace AuctionSys.Domain.Interfaces;

public interface IItemRepository : IAsyncRepository<Item>
{
    IQueryable<Item> GetQueryable();
    Task<(IReadOnlyList<Item> Items, int TotalCount, string? NextCursor)> GetMarketplaceItemsAsync(
        decimal? minPrice, decimal? maxPrice, string? searchTerm,
        decimal? minFloat, decimal? maxFloat, int? patternIndex, SkinExterior? exterior, bool? isStatTrak, bool? hasStickers,
        string? sortBy, string? cursor, int pageSize);
    Task<(IReadOnlyList<Item> Items, int TotalCount, string? NextCursor)> GetPagedItemsAsync(
        string? cursor, int pageSize, Guid? categoryId, string? search);
}
