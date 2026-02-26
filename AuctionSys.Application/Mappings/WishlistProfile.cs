using AutoMapper;
using AuctionSys.Application.DTOs.Wishlist;
using AuctionSys.Domain.Entities;

namespace AuctionSys.Application.Mappings;

public class WishlistProfile : Profile
{
    public WishlistProfile()
    {
        CreateMap<Wishlist, WishlistDto>();
    }
}
