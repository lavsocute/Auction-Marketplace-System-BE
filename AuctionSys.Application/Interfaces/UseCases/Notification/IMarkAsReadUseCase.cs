using AuctionSys.Application.Common.Models;

namespace AuctionSys.Application.Interfaces.UseCases.Notification;

public interface IMarkAsReadUseCase
{
    Task<ApiResponse<string>> ExecuteAsync(Guid userId, Guid notificationId);
}
