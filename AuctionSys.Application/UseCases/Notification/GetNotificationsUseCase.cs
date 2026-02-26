using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Notification;
using AuctionSys.Application.Interfaces.UseCases.Notification;
using AuctionSys.Domain.Interfaces;
using AutoMapper;

namespace AuctionSys.Application.UseCases.Notification;

public class GetNotificationsUseCase : IGetNotificationsUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetNotificationsUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResponse<NotificationDto>>> ExecuteAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
    {
        var allNotifications = await _unitOfWork.Notifications.GetAsync(n => n.UserId == userId);
        
        var totalCount = allNotifications.Count;

        var pagedNotifications = allNotifications
            .OrderByDescending(n => n.Id) // Id is sequential or should we use CreatedAt if available? Let's use Id or skip order if not strictly needed
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtos = _mapper.Map<List<NotificationDto>>(pagedNotifications);

        return ApiResponse<PagedResponse<NotificationDto>>.Success(new PagedResponse<NotificationDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        }, "Lấy danh sách thông báo thành công");
    }
}
