using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Auction;

namespace AuctionSys.Application.Interfaces.UseCases.Auction;

public interface IGetAuctionsUseCase
{
    Task<ApiResponse<PagedResponse<AuctionDto>>> ExecuteAsync(int pageNumber = 1, int pageSize = 10, string? status = null);
}
