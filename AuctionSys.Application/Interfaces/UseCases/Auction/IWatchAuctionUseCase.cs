using AuctionSys.Application.Common.Models;

namespace AuctionSys.Application.Interfaces.UseCases.Auction;

public interface IWatchAuctionUseCase
{
    Task<ApiResponse<string>> ExecuteAsync(Guid userId, Guid auctionId);
}
