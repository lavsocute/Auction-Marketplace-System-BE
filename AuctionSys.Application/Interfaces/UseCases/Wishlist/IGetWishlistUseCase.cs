using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Wishlist;

namespace AuctionSys.Application.Interfaces.UseCases.Wishlist;

public interface IGetWishlistUseCase
{
    Task<ApiResponse<IEnumerable<WishlistDto>>> ExecuteAsync(Guid userId);
}
