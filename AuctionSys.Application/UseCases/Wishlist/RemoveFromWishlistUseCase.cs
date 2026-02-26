using AuctionSys.Application.Common.Models;
using AuctionSys.Application.Interfaces.UseCases.Wishlist;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Wishlist;

public class RemoveFromWishlistUseCase : IRemoveFromWishlistUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveFromWishlistUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<string>> ExecuteAsync(Guid userId, Guid itemId)
    {
        var existing = await _unitOfWork.Wishlists.GetAsync(w => w.UserId == userId && w.ItemId == itemId);
        var wishlistItem = existing.FirstOrDefault();
        
        if (wishlistItem == null)
        {
            return ApiResponse<string>.Fail("Sản phẩm không có trong danh sách yêu thích.", 404);
        }

        await _unitOfWork.Wishlists.DeleteAsync(wishlistItem);
        await _unitOfWork.CompleteAsync();

        return ApiResponse<string>.Success("Đã xóa khỏi danh sách yêu thích.");
    }
}
