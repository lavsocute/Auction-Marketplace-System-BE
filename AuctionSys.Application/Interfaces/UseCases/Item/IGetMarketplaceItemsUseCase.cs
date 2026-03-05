using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Item;

namespace AuctionSys.Application.Interfaces.UseCases.Item;

public interface IGetMarketplaceItemsUseCase
{
    Task<ApiResponse<PagedResponse<MarketplaceItemDto>>> ExecuteAsync(MarketplaceItemFilterDto filter);
}
