using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Auction;
using AuctionSys.Application.Interfaces.UseCases.Auction;
using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;
using StackExchange.Redis;
using AutoMapper;

namespace AuctionSys.Application.UseCases.Auction;

public class PlaceBidUseCase : IPlaceBidUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConnectionMultiplexer _redis;
    private readonly IMapper _mapper;

    public PlaceBidUseCase(IUnitOfWork unitOfWork, IConnectionMultiplexer redis, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _redis = redis;
        _mapper = mapper;
    }

    public async Task<ApiResponse<BidDto>> ExecuteAsync(Guid bidderId, Guid auctionId, PlaceBidDto request)
    {
        var db = _redis.GetDatabase();
        var lockKey = $"lock_auction_{auctionId}";
        var lockToken = Guid.NewGuid().ToString();

        // 1. Redis Distributed Lock to prevent race conditions during bidding
        if (!await db.LockTakeAsync(lockKey, lockToken, TimeSpan.FromSeconds(10)))
        {
            return ApiResponse<BidDto>.Fail("Hệ thống đang xử lý một lượt ra giá khác. Hãy thử lại.", 409);
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var auction = await _unitOfWork.Auctions.GetByIdAsync(auctionId);
            if (auction == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<BidDto>.Fail("Phiên đấu giá không tồn tại.", 404);
            }

            if (auction.Status != AuctionStatus.Active || auction.EndTime <= DateTime.UtcNow)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<BidDto>.Fail("Phiên đấu giá đã kết thúc hoặc không hoạt động.", 400);
            }

            var item = await _unitOfWork.Items.GetByIdAsync(auction.ItemId);
            if (item != null && item.SellerId == bidderId)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<BidDto>.Fail("Bạn không thể tự ra giá cho sản phẩm của mình.", 400);
            }

            if (request.Amount <= auction.CurrentPrice)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<BidDto>.Fail($"Mức giá phải lớn hơn giá hiện tại ({auction.CurrentPrice:N0} VNĐ).", 400);
            }

            var bidderWallets = await _unitOfWork.Wallets.GetAsync(w => w.UserId == bidderId);
            var bidderWallet = bidderWallets.FirstOrDefault();

            var previousWinnerId = auction.WinnerId;
            var previousPrice = auction.CurrentPrice;

            // Tính số tiền khả dụng thật sự:
            // Nếu tự mình nâng giá của chính mình, số tiền Frozen cũ có thể được tái sử dụng
            decimal actualAvailableBalance = bidderWallet?.Balance ?? 0;
            if (previousWinnerId.HasValue && previousWinnerId.Value == bidderId)
            {
                actualAvailableBalance += previousPrice;
            }

            if (bidderWallet == null || actualAvailableBalance < request.Amount)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<BidDto>.Fail("Số dư trong ví không đủ để ra giá mức này.", 400);
            }

            // 2. Xử lý ví người Bid trước đó (Hoàn trả tiền bị đóng băng do bị vượt giá)
            if (previousWinnerId.HasValue && previousWinnerId.Value != bidderId)
            {
                var prevBidderWallets = await _unitOfWork.Wallets.GetAsync(w => w.UserId == previousWinnerId.Value);
                var prevBidderWallet = prevBidderWallets.FirstOrDefault();

                if (prevBidderWallet != null)
                {
                    prevBidderWallet.FrozenBalance -= previousPrice;
                    prevBidderWallet.Balance += previousPrice;
                    
                    await _unitOfWork.WalletTransactions.AddAsync(new WalletTransaction
                    {
                        WalletId = prevBidderWallet.Id,
                        Amount = previousPrice,
                        Type = TransactionType.BidUnfreeze,
                        Description = $"Hoàn trả {previousPrice:N0} VNĐ do có người ra giá cao hơn trong phiên '{item?.Title}'"
                    });

                    // Thông báo bị vượt giá
                    await _unitOfWork.Notifications.AddAsync(new AuctionSys.Domain.Entities.Notification
                    {
                        UserId = previousWinnerId.Value,
                        Type = NotificationType.Outbid,
                        Title = "Bạn đã bị vượt giá!",
                        Message = $"Có người vừa ra giá {request.Amount:N0} VNĐ cho sản phẩm '{item?.Title}'. Hãy ra giá cao hơn để giành chiến thắng!",
                        ReferenceId = auctionId.ToString()
                    });
                }
            }
            else if (previousWinnerId.HasValue && previousWinnerId.Value == bidderId)
            {
                // Tự hoàn lại khoản tiền đóng băng cũ của chính mình trước khi trừ theo lượng giá mới
                bidderWallet.FrozenBalance -= previousPrice;
                bidderWallet.Balance += previousPrice;
            }

            // 3. Xử lý ví người Bid hiện tại (Đóng băng tiền theo mức giá mới)
            bidderWallet.Balance -= request.Amount;
            bidderWallet.FrozenBalance += request.Amount;
            
            await _unitOfWork.WalletTransactions.AddAsync(new WalletTransaction
            {
                WalletId = bidderWallet.Id,
                Amount = request.Amount,
                Type = TransactionType.BidFreeze,
                Description = $"Đóng băng {request.Amount:N0} VNĐ chờ kết quả phiên đấu giá '{item?.Title}'"
            });

            // 4. Cập nhật trạng thái Auction và tạo Bid
            auction.CurrentPrice = request.Amount;
            auction.WinnerId = bidderId;
            await _unitOfWork.Auctions.UpdateAsync(auction);

            var bid = new Bid
            {
                AuctionId = auctionId,
                BidderId = bidderId,
                Amount = request.Amount,
                BidTime = DateTime.UtcNow
            };
            await _unitOfWork.Bids.AddAsync(bid);

            // 5. Thông báo cho những người đang Watch (AuctionWatcher)
            var watchers = await _unitOfWork.AuctionWatchers.GetAsync(w => w.AuctionId == auctionId);
            foreach (var watcher in watchers)
            {
                // Không thông báo cho người vừa bid
                if (watcher.UserId == bidderId) continue;
                // Người bị vượt giá đã có thông báo riêng ở trên
                if (previousWinnerId.HasValue && watcher.UserId == previousWinnerId.Value) continue;

                await _unitOfWork.Notifications.AddAsync(new AuctionSys.Domain.Entities.Notification
                {
                    UserId = watcher.UserId,
                    Type = NotificationType.Outbid, // Có thể tao thêm Type mới như NewBid, tạm dùng Outbid
                    Title = "Có lượt ra giá mới!",
                    Message = $"Sản phẩm '{item?.Title}' mà bạn theo dõi vừa có lượt ra giá mới: {request.Amount:N0} VNĐ.",
                    ReferenceId = auctionId.ToString()
                });
            }

            await _unitOfWork.CommitTransactionAsync();
            await _unitOfWork.CompleteAsync();

            var bidDto = _mapper.Map<BidDto>(bid);
            return ApiResponse<BidDto>.Success(bidDto, "Ra giá thành công!");
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
        finally
        {
            // 6. Release Redis Lock
            await db.LockReleaseAsync(lockKey, lockToken);
        }
    }
}
