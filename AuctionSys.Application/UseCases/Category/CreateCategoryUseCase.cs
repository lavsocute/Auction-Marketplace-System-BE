using AutoMapper;
using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Category;
using AuctionSys.Application.Interfaces.UseCases.Category;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Category;

public class CreateCategoryUseCase : ICreateCategoryUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCategoryUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CategoryDto>> ExecuteAsync(CreateCategoryDto request)
    {
        var categoryEntity = _mapper.Map<AuctionSys.Domain.Entities.Category>(request);

        await _unitOfWork.Categories.AddAsync(categoryEntity);
        var success = await _unitOfWork.CompleteAsync() > 0;

        if (!success)
        {
            return ApiResponse<CategoryDto>.Fail("Tạo danh mục thất bại. Có lỗi khi lưu vào cơ sở dữ liệu.");
        }

        var categoryDto = _mapper.Map<CategoryDto>(categoryEntity);
        return ApiResponse<CategoryDto>.Success(categoryDto, "Hệ thống đã tạo danh mục thành công.", 201);
    }
}
