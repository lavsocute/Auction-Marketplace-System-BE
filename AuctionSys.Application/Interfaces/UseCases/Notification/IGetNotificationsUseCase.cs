using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Notification;

namespace AuctionSys.Application.Interfaces.UseCases.Notification;

public interface IGetNotificationsUseCase
{
    Task<ApiResponse<PagedResponse<NotificationDto>>> ExecuteAsync(Guid userId, int pageNumber = 1, int pageSize = 10);
}
