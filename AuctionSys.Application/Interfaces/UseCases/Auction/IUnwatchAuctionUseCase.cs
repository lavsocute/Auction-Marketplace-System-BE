using AuctionSys.Application.Common.Models;

namespace AuctionSys.Application.Interfaces.UseCases.Auction;

public interface IUnwatchAuctionUseCase
{
    Task<ApiResponse<string>> ExecuteAsync(Guid userId, Guid auctionId);
}
