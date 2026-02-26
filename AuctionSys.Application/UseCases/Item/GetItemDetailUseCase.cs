using AutoMapper;
using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Item;
using AuctionSys.Application.Interfaces.UseCases.Item;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Item;

public class GetItemDetailUseCase : IGetItemDetailUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetItemDetailUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ItemDetailDto>> ExecuteAsync(Guid itemId)
    {
        // Ideally we need a method to Include Seller, but Generic Repo GetByIdAsync might not have includes.
        // We'll fetch Item and then fetch Seller manually if we don't have GetWithIncludes in repo.
        var item = await _unitOfWork.Items.GetByIdAsync(itemId);
        if (item == null)
        {
            return ApiResponse<ItemDetailDto>.Fail("Sản phẩm không tồn tại.", 404);
        }

        var seller = await _unitOfWork.Users.GetByIdAsync(item.SellerId);

        var itemDto = _mapper.Map<ItemDetailDto>(item);
        if (seller != null)
        {
            itemDto.Seller = _mapper.Map<AuctionSys.Application.DTOs.User.UserProfileDto>(seller);
        }

        return ApiResponse<ItemDetailDto>.Success(itemDto);
    }
}
