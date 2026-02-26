using AutoMapper;
using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Category;
using AuctionSys.Application.Interfaces.UseCases.Category;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Category;

public class GetAllCategoriesUseCase : IGetAllCategoriesUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCategoriesUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<CategoryDto>>> ExecuteAsync()
    {
        var categories = await _unitOfWork.Categories.ListAllAsync();
        var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);

        return ApiResponse<IEnumerable<CategoryDto>>.Success(categoryDtos, "Lấy danh sách danh mục thành công.");
    }
}
