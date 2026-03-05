using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Bot;
using AuctionSys.Application.DTOs.Item;
using AuctionSys.Application.Interfaces.UseCases.Item;
using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Item;

public class GetMarketplaceItemsUseCase : IGetMarketplaceItemsUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMarketplaceItemsUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResponse<MarketplaceItemDto>>> ExecuteAsync(MarketplaceItemFilterDto filter)
    {
        // Delegate complex query completely to Repository to keep Application layer free of EF Core references
        var (items, totalCount) = await _unitOfWork.Items.GetMarketplaceItemsAsync(
            filter.MinPrice, filter.MaxPrice, filter.SearchTerm,
            filter.MinFloat, filter.MaxFloat, filter.PatternIndex, filter.Exterior, filter.IsStatTrak, filter.HasStickers,
            filter.SortBy?.ToString(), filter.PageNumber, filter.PageSize
        );

        // 5. Map to DTO
        var dtos = items.Select(i => new MarketplaceItemDto
        {
            Id = i.Id,
            Title = i.Title,
            Description = i.Description,
            ImageUrl = i.ImageUrl,
            Price = i.Price,
            ListType = i.ListType,
            Status = i.Status,
            SellerId = i.SellerId,
            CategoryId = i.CategoryId,
            CreatedAt = i.CreatedAt,
            // Get TradeLock from the active BotInventory record if any
            TradeLockedUntil = i.BotInventories.FirstOrDefault()?.TradeLockedUntil,
            SkinMetadata = i.SkinMetadata == null ? null : new SkinMetadataDto
            {
                Id = i.SkinMetadata.Id,
                Exterior = i.SkinMetadata.Exterior,
                FloatValue = i.SkinMetadata.FloatValue ?? 0,
                PatternIndex = i.SkinMetadata.PatternIndex ?? 0,
                StickersJson = i.SkinMetadata.StickersJson,
                NameTag = i.SkinMetadata.NameTag,
                StatTrak = i.SkinMetadata.IsStatTrak
            }
        }).ToList();

        var pagedResponse = new PagedResponse<MarketplaceItemDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };

        return ApiResponse<PagedResponse<MarketplaceItemDto>>.Success(pagedResponse);
    }
}
