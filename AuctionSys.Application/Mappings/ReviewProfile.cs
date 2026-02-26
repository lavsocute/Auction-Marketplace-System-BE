using AuctionSys.Application.DTOs.Review;
using AutoMapper;

namespace AuctionSys.Application.Mappings;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<Domain.Entities.Review, ReviewDto>();
    }
}
