using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Auction;

namespace AuctionSys.Application.Interfaces.UseCases.Auction;

public interface ICreateAuctionUseCase
{
    Task<ApiResponse<AuctionDto>> ExecuteAsync(Guid sellerId, CreateAuctionDto request);
}
