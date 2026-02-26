using AuctionSys.Application.Common.Models;
using AuctionSys.Application.Interfaces.UseCases.Auction;
using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Auction;

public class WatchAuctionUseCase : IWatchAuctionUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public WatchAuctionUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<string>> ExecuteAsync(Guid userId, Guid auctionId)
    {
        var auction = await _unitOfWork.Auctions.GetByIdAsync(auctionId);
        if (auction == null)
        {
            return ApiResponse<string>.Fail("Phiên đấu giá không tồn tại.", 404);
        }

        var existingWatch = await _unitOfWork.AuctionWatchers.GetAsync(w => w.UserId == userId && w.AuctionId == auctionId);
        if (existingWatch.Any())
        {
            return ApiResponse<string>.Fail("Bạn đã theo dõi phiên đấu giá này rồi.", 400);
        }

        var watcher = new AuctionWatcher
        {
            UserId = userId,
            AuctionId = auctionId,
            WatchedAt = DateTime.UtcNow
        };

        await _unitOfWork.AuctionWatchers.AddAsync(watcher);
        await _unitOfWork.CompleteAsync();

        return ApiResponse<string>.Success("Đã thêm vào danh sách theo dõi phiên đấu giá.");
    }
}
