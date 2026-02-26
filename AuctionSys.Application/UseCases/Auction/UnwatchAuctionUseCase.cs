using AuctionSys.Application.Common.Models;
using AuctionSys.Application.Interfaces.UseCases.Auction;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Auction;

public class UnwatchAuctionUseCase : IUnwatchAuctionUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public UnwatchAuctionUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<string>> ExecuteAsync(Guid userId, Guid auctionId)
    {
        var existingWatch = await _unitOfWork.AuctionWatchers.GetAsync(w => w.UserId == userId && w.AuctionId == auctionId);
        var watcher = existingWatch.FirstOrDefault();

        if (watcher == null)
        {
            return ApiResponse<string>.Fail("Bạn chưa theo dõi phiên đấu giá này.", 404);
        }

        await _unitOfWork.AuctionWatchers.DeleteAsync(watcher);
        await _unitOfWork.CompleteAsync();

        return ApiResponse<string>.Success("Đã bỏ theo dõi phiên đấu giá.");
    }
}
