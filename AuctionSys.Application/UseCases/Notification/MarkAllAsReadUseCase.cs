using AuctionSys.Application.Common.Models;
using AuctionSys.Application.Interfaces.UseCases.Notification;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Notification;

public class MarkAllAsReadUseCase : IMarkAllAsReadUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkAllAsReadUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<string>> ExecuteAsync(Guid userId)
    {
        var unreadNotifications = await _unitOfWork.Notifications.GetAsync(n => n.UserId == userId && !n.IsRead);
        
        if (!unreadNotifications.Any())
            return ApiResponse<string>.Success("Tất cả thông báo đã được đọc.");

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            await _unitOfWork.Notifications.UpdateAsync(notification);
        }

        await _unitOfWork.CompleteAsync();

        return ApiResponse<string>.Success($"Đã đánh dấu {unreadNotifications.Count()} thông báo là đã đọc.");
    }
}
