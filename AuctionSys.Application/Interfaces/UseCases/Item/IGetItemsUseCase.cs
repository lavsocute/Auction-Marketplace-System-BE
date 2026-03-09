using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Item;

namespace AuctionSys.Application.Interfaces.UseCases.Item;

public interface IGetItemsUseCase
{
    Task<ApiResponse<PagedResponse<ItemDto>>> ExecuteAsync(
        string? cursor = null, 
        int pageSize = 10, 
        Guid? categoryId = null, 
        string? search = null);
}
