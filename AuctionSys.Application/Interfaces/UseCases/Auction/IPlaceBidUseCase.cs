using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Auction;

namespace AuctionSys.Application.Interfaces.UseCases.Auction;

public interface IPlaceBidUseCase
{
    Task<ApiResponse<BidDto>> ExecuteAsync(Guid bidderId, Guid auctionId, PlaceBidDto request);
}
