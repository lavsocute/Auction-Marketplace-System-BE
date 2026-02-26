using AuctionSys.Application.DTOs.Notification;
using AutoMapper;

namespace AuctionSys.Application.Mappings;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Domain.Entities.Notification, NotificationDto>();
    }
}
