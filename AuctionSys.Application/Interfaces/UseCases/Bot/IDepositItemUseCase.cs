using AuctionSys.Application.Common;
using AuctionSys.Application.DTOs.Bot;

namespace AuctionSys.Application.Interfaces.UseCases.Bot;

public interface IDepositItemUseCase
{
    Task<ApiResponse<string>> ExecuteAsync(Guid botId, Guid sellerId, DepositItemRequestDto request);
}
