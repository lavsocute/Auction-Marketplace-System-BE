using AuctionSys.Application.Common.Models;

namespace AuctionSys.Application.Interfaces.UseCases.Wishlist;

public interface IAddToWishlistUseCase
{
    Task<ApiResponse<string>> ExecuteAsync(Guid userId, Guid itemId);
}
