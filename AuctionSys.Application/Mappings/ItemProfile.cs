using AutoMapper;
using AuctionSys.Application.DTOs.Item;
using AuctionSys.Domain.Entities;

namespace AuctionSys.Application.Mappings;

public class ItemProfile : Profile
{
    public ItemProfile()
    {
        CreateMap<Item, ItemDto>();
        CreateMap<Item, ItemDetailDto>();
        CreateMap<CreateItemDto, Item>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 
                src.ListType == AuctionSys.Domain.Enums.ListType.Auction 
                    ? AuctionSys.Domain.Enums.ItemStatus.Available 
                    : AuctionSys.Domain.Enums.ItemStatus.Available));
    }
}
