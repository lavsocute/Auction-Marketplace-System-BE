using AuctionSys.Application.Common.Models;

namespace AuctionSys.Application.Interfaces.UseCases.Notification;

public interface IMarkAllAsReadUseCase
{
    Task<ApiResponse<string>> ExecuteAsync(Guid userId);
}
