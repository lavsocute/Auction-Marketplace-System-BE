using AutoMapper;
using AuctionSys.Application.DTOs.Auction;
using AuctionSys.Domain.Entities;

namespace AuctionSys.Application.Mappings;

public class AuctionProfile : Profile
{
    public AuctionProfile()
    {
        CreateMap<Auction, AuctionDto>();
        CreateMap<Bid, BidDto>();
    }
}
