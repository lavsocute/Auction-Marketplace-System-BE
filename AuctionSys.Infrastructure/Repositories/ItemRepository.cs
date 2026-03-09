using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;
using AuctionSys.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AuctionSys.Infrastructure.Repositories;

public class ItemRepository : AsyncRepository<Item>, IItemRepository
{
    private readonly AppDbContext _context;

    public ItemRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public IQueryable<Item> GetQueryable()
    {
        return _context.Items.AsQueryable();
    }

    public async Task<(IReadOnlyList<Item> Items, int TotalCount, string? NextCursor)> GetMarketplaceItemsAsync(
        decimal? minPrice, decimal? maxPrice, string? searchTerm,
        decimal? minFloat, decimal? maxFloat, int? patternIndex, SkinExterior? exterior, bool? isStatTrak, bool? hasStickers,
        string? sortBy, string? cursor, int pageSize)
    {
        var query = _context.Items
            .AsNoTracking()
            .Include(i => i.SkinMetadata)
            .Include(i => i.BotInventories)
            .AsSplitQuery()
            .Where(i => i.Status == ItemStatus.InBotInventory || i.Status == ItemStatus.TradeLocked);

        // Apply Filters
        if (minPrice.HasValue) query = query.Where(i => i.Price >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(i => i.Price <= maxPrice.Value);
        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(i => i.Title.ToLower().Contains(searchTerm.ToLower()));

        // SkinMetadata Filters
        if (minFloat.HasValue) query = query.Where(i => i.SkinMetadata != null && i.SkinMetadata.FloatValue >= minFloat.Value);
        if (maxFloat.HasValue) query = query.Where(i => i.SkinMetadata != null && i.SkinMetadata.FloatValue <= maxFloat.Value);
        if (exterior.HasValue) query = query.Where(i => i.SkinMetadata != null && i.SkinMetadata.Exterior == exterior.Value);
        if (patternIndex.HasValue) query = query.Where(i => i.SkinMetadata != null && i.SkinMetadata.PatternIndex == patternIndex.Value);
        if (isStatTrak.HasValue) query = query.Where(i => i.SkinMetadata != null && i.SkinMetadata.IsStatTrak == isStatTrak.Value);
        if (hasStickers.HasValue && hasStickers.Value)
            query = query.Where(i => i.SkinMetadata != null && !string.IsNullOrEmpty(i.SkinMetadata.StickersJson) && i.SkinMetadata.StickersJson != "[]");

        // Apply Sorting
        query = sortBy switch
        {
            "PriceAsc" => query.OrderBy(i => i.Price),
            "PriceDesc" => query.OrderByDescending(i => i.Price),
            "FloatAsc" => query.OrderBy(i => i.SkinMetadata != null ? i.SkinMetadata.FloatValue : 1m),
            _ => query.OrderByDescending(i => i.CreatedAt)
        };

        // Apply Cursor (Instead of Offset/Skip)
        if (!string.IsNullOrWhiteSpace(cursor) && DateTime.TryParse(cursor, out var parsedCursor))
        {
            query = sortBy switch
            {
                "PriceAsc" => query.Where(i => i.Price > 0), // Not ideal for cursor, fallback needed for complex sorting
                _ => query.Where(i => i.CreatedAt < parsedCursor)
            };
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Take(pageSize)
            .ToListAsync();

        string? nextCursor = items.Count == pageSize ? items.Last().CreatedAt.ToString("o") : null;

        return (items, totalCount, nextCursor);
    }

    public async Task<(IReadOnlyList<Item> Items, int TotalCount, string? NextCursor)> GetPagedItemsAsync(
        string? cursor, int pageSize, Guid? categoryId, string? search)
    {
        var query = _context.Items.AsNoTracking();

        if (categoryId.HasValue)
        {
            query = query.Where(i => i.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim().ToLower();
            query = query.Where(i => i.Title.ToLower().Contains(searchLower) || 
                                     i.Description.ToLower().Contains(searchLower));
        }

        // Apply Cursor Pagination
        if (!string.IsNullOrWhiteSpace(cursor) && DateTime.TryParse(cursor, out var parsedCursor))
        {
            query = query.Where(i => i.CreatedAt < parsedCursor);
        }

        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderByDescending(i => i.CreatedAt)
            .Take(pageSize)
            .ToListAsync();

        string? nextCursor = items.Count == pageSize ? items.Last().CreatedAt.ToString("o") : null;

        return (items, totalCount, nextCursor);
    }
}
