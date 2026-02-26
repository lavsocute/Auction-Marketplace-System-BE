using AuctionSys.Application.Common.Models;
using AuctionSys.Application.Interfaces.UseCases.Notification;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Notification;

public class MarkAsReadUseCase : IMarkAsReadUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkAsReadUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<string>> ExecuteAsync(Guid userId, Guid notificationId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
        if (notification == null)
            return ApiResponse<string>.Fail("Thông báo không tồn tại.", 404);

        if (notification.UserId != userId)
            return ApiResponse<string>.Fail("Bạn không có quyền truy cập thông báo này.", 403);

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            await _unitOfWork.Notifications.UpdateAsync(notification);
            await _unitOfWork.CompleteAsync();
        }

        return ApiResponse<string>.Success("Đánh dấu đã đọc thành công.");
    }
}
