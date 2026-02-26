using AutoMapper;
using AuctionSys.Application.DTOs.Category;
using AuctionSys.Domain.Entities;

namespace AuctionSys.Application.Mappings;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<CreateCategoryDto, Category>();
    }
}
