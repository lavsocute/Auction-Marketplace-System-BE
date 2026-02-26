using AutoMapper;
using AuctionSys.Application.DTOs.User;
using AuctionSys.Domain.Entities;

namespace AuctionSys.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserProfileDto>();
    }
}
