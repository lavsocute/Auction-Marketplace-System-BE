using AuctionSys.Application.Common.Models;

namespace AuctionSys.Application.Interfaces.UseCases.Wishlist;

public interface IRemoveFromWishlistUseCase
{
    Task<ApiResponse<string>> ExecuteAsync(Guid userId, Guid itemId);
}
