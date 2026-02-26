using AutoMapper;
using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Item;
using AuctionSys.Application.Interfaces.UseCases.Item;
using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Item;

public class CreateItemUseCase : ICreateItemUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateItemUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ItemDto>> ExecuteAsync(Guid sellerId, CreateItemDto request)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
        if (category == null)
        {
            return ApiResponse<ItemDto>.Fail("Danh mục không tồn tại.");
        }

        var item = _mapper.Map<AuctionSys.Domain.Entities.Item>(request);
        item.SellerId = sellerId;

        await _unitOfWork.Items.AddAsync(item);
        await _unitOfWork.CompleteAsync();

        var dto = _mapper.Map<ItemDto>(item);
        return ApiResponse<ItemDto>.Success(dto, "Đăng bán sản phẩm thành công.", 201);
    }
}
