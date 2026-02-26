using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Item;

namespace AuctionSys.Application.Interfaces.UseCases.Item;

public interface ICreateItemUseCase
{
    Task<ApiResponse<ItemDto>> ExecuteAsync(Guid sellerId, CreateItemDto request);
}
