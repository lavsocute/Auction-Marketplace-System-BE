using AuctionSys.Application.Interfaces.UseCases.Auction;
using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;
using AuctionSys.Domain.Entities;
using StackExchange.Redis;

namespace AuctionSys.Application.UseCases.Auction;

public class CloseAuctionUseCase : ICloseAuctionUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConnectionMultiplexer _redis;

    public CloseAuctionUseCase(IUnitOfWork unitOfWork, IConnectionMultiplexer redis)
    {
        _unitOfWork = unitOfWork;
        _redis = redis;
    }

    public async Task ExecuteAsync(Guid auctionId)
    {
        var db = _redis.GetDatabase();
        var lockKey = $"lock_auction_close_{auctionId}";
        var lockToken = Guid.NewGuid().ToString();

        // 1. Redis Lock to ensure this job runs only once concurrently
        if (!await db.LockTakeAsync(lockKey, lockToken, TimeSpan.FromSeconds(30)))
        {
            return;
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var auction = await _unitOfWork.Auctions.GetByIdAsync(auctionId);
            if (auction == null || auction.Status != AuctionStatus.Active)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return;
            }

            var item = await _unitOfWork.Items.GetByIdAsync(auction.ItemId);
            if (item == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return;
            }

            if (auction.WinnerId.HasValue)
            {
                // Có người thắng
                var winnerWallets = await _unitOfWork.Wallets.GetAsync(w => w.UserId == auction.WinnerId.Value);
                var winnerWallet = winnerWallets.FirstOrDefault();

                var sellerWallets = await _unitOfWork.Wallets.GetAsync(w => w.UserId == item.SellerId);
                var sellerWallet = sellerWallets.FirstOrDefault();
                
                if (sellerWallet == null)
                {
                    sellerWallet = new AuctionSys.Domain.Entities.Wallet { UserId = item.SellerId, Balance = 0, FrozenBalance = 0 };
                    await _unitOfWork.Wallets.AddAsync(sellerWallet);
                }

                if (winnerWallet != null)
                {
                    // Trừ tiền đóng băng của Winner
                    winnerWallet.FrozenBalance -= auction.CurrentPrice;

                    // Chuyển tiền cho Seller
                    sellerWallet.Balance += auction.CurrentPrice;

                    // Ghi Transaction
                    await _unitOfWork.WalletTransactions.AddAsync(new WalletTransaction
                    {
                        WalletId = winnerWallet.Id,
                        Amount = -auction.CurrentPrice,
                        Type = TransactionType.AuctionWin,
                        Description = $"Chi trả {auction.CurrentPrice:N0} VNĐ cho phiên đấu giá '{item.Title}'"
                    });

                    await _unitOfWork.WalletTransactions.AddAsync(new WalletTransaction
                    {
                        WalletId = sellerWallet.Id,
                        Amount = auction.CurrentPrice,
                        Type = TransactionType.Sale,
                        Description = $"Thu tiền từ phiên đấu giá '{item.Title}' (+{auction.CurrentPrice:N0} VNĐ)"
                    });
                }
                
                // Cập nhật trạng thái item
                item.Status = ItemStatus.Sold;

                // Cập nhật trạng thái auction
                auction.Status = AuctionStatus.Ended;
                
                // Tạo Order
                var order = new AuctionSys.Domain.Entities.Order
                {
                    BuyerId = auction.WinnerId.Value,
                    ItemId = item.Id,
                    TotalPrice = auction.CurrentPrice,
                    Status = OrderStatus.Completed
                };
                await _unitOfWork.Orders.AddAsync(order);

                // Notifications
                await _unitOfWork.Notifications.AddAsync(new AuctionSys.Domain.Entities.Notification
                {
                    UserId = auction.WinnerId.Value,
                    Type = NotificationType.AuctionWon,
                    Title = "Chúc mừng! Bạn đã thắng phiên đấu giá.",
                    Message = $"Bạn đã thắng phiên đấu giá sản phẩm '{item.Title}' với mức giá {auction.CurrentPrice:N0} VNĐ.",
                    ReferenceId = auction.Id.ToString()
                });

                await _unitOfWork.Notifications.AddAsync(new AuctionSys.Domain.Entities.Notification
                {
                    UserId = item.SellerId,
                    Type = NotificationType.AuctionEnded,
                    Title = "Phiên đấu giá kết thúc thành công!",
                    Message = $"Sản phẩm '{item.Title}' đã được bán với mức giá {auction.CurrentPrice:N0} VNĐ.",
                    ReferenceId = auction.Id.ToString()
                });
            }
            else
            {
                // Không có ai bid
                auction.Status = AuctionStatus.Ended;
                item.Status = ItemStatus.Available; // Reset lại để có thể mang đi đấu giá tiếp hoặc bán thẳng nếu đổi Type

                await _unitOfWork.Notifications.AddAsync(new AuctionSys.Domain.Entities.Notification
                {
                    UserId = item.SellerId,
                    Type = NotificationType.AuctionEnded,
                    Title = "Phiên đấu giá kết thúc mà không có lượt ra giá.",
                    Message = $"Sản phẩm '{item.Title}' đã không thu hút lượt ra giá nào và đã quay về trạng thái khả dụng.",
                    ReferenceId = auction.Id.ToString()
                });
            }

            await _unitOfWork.Items.UpdateAsync(item);
            await _unitOfWork.Auctions.UpdateAsync(auction);

            // Thông báo cho tất cả Watchers
            var watchers = await _unitOfWork.AuctionWatchers.GetAsync(w => w.AuctionId == auctionId);
            foreach (var watcher in watchers)
            {
                if (auction.WinnerId.HasValue && watcher.UserId == auction.WinnerId.Value) continue;
                if (watcher.UserId == item.SellerId) continue;

                await _unitOfWork.Notifications.AddAsync(new AuctionSys.Domain.Entities.Notification
                {
                    UserId = watcher.UserId,
                    Type = NotificationType.AuctionEnded,
                    Title = "Phiên đấu giá đã kết thúc.",
                    Message = $"Phiên đấu giá sản phẩm '{item.Title}' mà bạn theo dõi đã kết thúc.",
                    ReferenceId = auction.Id.ToString()
                });
            }

            await _unitOfWork.CommitTransactionAsync();
            await _unitOfWork.CompleteAsync();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
        finally
        {
            await db.LockReleaseAsync(lockKey, lockToken);
        }
    }
}
