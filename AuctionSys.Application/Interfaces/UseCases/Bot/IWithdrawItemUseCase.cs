using AuctionSys.Application.Common.Models;

namespace AuctionSys.Application.Interfaces.UseCases.Bot;

public interface IWithdrawItemUseCase
{
    Task<ApiResponse<bool>> ExecuteAsync(Guid botId, Guid itemId, Guid userId);
}
