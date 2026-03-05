using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Enums;
using System.Linq.Expressions;

namespace AuctionSys.Domain.Interfaces;

public interface IItemRepository : IAsyncRepository<Item>
{
    IQueryable<Item> GetQueryable();
    Task<(IReadOnlyList<Item> Items, int TotalCount)> GetMarketplaceItemsAsync(
        decimal? minPrice, decimal? maxPrice, string? searchTerm,
        decimal? minFloat, decimal? maxFloat, int? patternIndex, SkinExterior? exterior, bool? isStatTrak, bool? hasStickers,
        string? sortBy, int pageNumber, int pageSize);
}
