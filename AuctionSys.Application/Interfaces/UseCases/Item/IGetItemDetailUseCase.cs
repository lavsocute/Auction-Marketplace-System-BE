using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Item;

namespace AuctionSys.Application.Interfaces.UseCases.Item;

public interface IGetItemDetailUseCase
{
    Task<ApiResponse<ItemDetailDto>> ExecuteAsync(Guid itemId);
}
