using AutoMapper;
using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Wishlist;
using AuctionSys.Application.Interfaces.UseCases.Wishlist;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Wishlist;

public class GetWishlistUseCase : IGetWishlistUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetWishlistUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<WishlistDto>>> ExecuteAsync(Guid userId)
    {
        var wishlists = await _unitOfWork.Wishlists.GetAsync(w => w.UserId == userId);
        
        // Cần Include Item info, nhưng repo get cơ bản chưa có. 
        // Thay vì viết query phức tạp, ta map trước rồi manual lấy item nếu list nhỏ.
        var dtos = _mapper.Map<IEnumerable<WishlistDto>>(wishlists).ToList();
        
        foreach (var dto in dtos)
        {
            var item = await _unitOfWork.Items.GetByIdAsync(dto.ItemId);
            if (item != null)
            {
                dto.Item = _mapper.Map<AuctionSys.Application.DTOs.Item.ItemDto>(item);
            }
        }

        return ApiResponse<IEnumerable<WishlistDto>>.Success(dtos);
    }
}
