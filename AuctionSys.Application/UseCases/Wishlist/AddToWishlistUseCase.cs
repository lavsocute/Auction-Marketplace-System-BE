using AuctionSys.Application.Common.Models;
using AuctionSys.Application.Interfaces.UseCases.Wishlist;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Wishlist;

public class AddToWishlistUseCase : IAddToWishlistUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public AddToWishlistUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<string>> ExecuteAsync(Guid userId, Guid itemId)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(itemId);
        if (item == null)
        {
            return ApiResponse<string>.Fail("Sản phẩm không tồn tại.", 404);
        }

        var existing = await _unitOfWork.Wishlists.GetAsync(w => w.UserId == userId && w.ItemId == itemId);
        if (existing.Any())
        {
            return ApiResponse<string>.Fail("Sản phẩm đã có trong danh sách yêu thích.", 400);
        }

        var wishlist = new AuctionSys.Domain.Entities.Wishlist
        {
            UserId = userId,
            ItemId = itemId
        };

        await _unitOfWork.Wishlists.AddAsync(wishlist);
        await _unitOfWork.CompleteAsync();

        return ApiResponse<string>.Success("Đã thêm vào danh sách yêu thích.");
    }
}
