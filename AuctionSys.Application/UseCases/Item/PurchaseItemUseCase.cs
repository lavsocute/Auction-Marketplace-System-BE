using AuctionSys.Application.Common.Models;
using AuctionSys.Application.Interfaces.UseCases.Item;
using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;
using StackExchange.Redis;

namespace AuctionSys.Application.UseCases.Item;

public class PurchaseItemUseCase : IPurchaseItemUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConnectionMultiplexer _redis;
    private readonly IWalletSignatureService _signatureService;

    public PurchaseItemUseCase(IUnitOfWork unitOfWork, IConnectionMultiplexer redis, IWalletSignatureService signatureService)
    {
        _unitOfWork = unitOfWork;
        _redis = redis;
        _signatureService = signatureService;
    }

    public async Task<ApiResponse<string>> ExecuteAsync(Guid buyerId, Guid itemId)
    {
        var db = _redis.GetDatabase();
        var lockKey = $"lock_item_{itemId}";
        var lockToken = Guid.NewGuid().ToString();

        // 1. Redis Lock
        if (!await db.LockTakeAsync(lockKey, lockToken, TimeSpan.FromSeconds(10)))
        {
            return ApiResponse<string>.Fail("Hệ thống đang xử lý giao dịch cho sản phẩm này. Hãy thử lại sau.", 409);
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var item = await _unitOfWork.Items.GetByIdAsync(itemId);
            if (item == null || item.ListType != ListType.FixedPrice || item.Status != ItemStatus.Available)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<string>.Fail("Sản phẩm không khả dụng để mua.", 400);
            }

            if (item.SellerId == buyerId)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<string>.Fail("Bạn không thể mua sản phẩm của chính mình.", 400);
            }

            // Lock ví trong code hoặc rely on DB transaction bounds (Isolation Level)
            var buyerWallets = await _unitOfWork.Wallets.GetAsync(w => w.UserId == buyerId);
            var buyerWallet = buyerWallets.FirstOrDefault();
            
            if (buyerWallet == null || buyerWallet.Balance < item.Price)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<string>.Fail("Số dư trong ví không đủ.", 400);
            }

            var sellerWallets = await _unitOfWork.Wallets.GetAsync(w => w.UserId == item.SellerId);
            var sellerWallet = sellerWallets.FirstOrDefault();
            if (sellerWallet == null)
            {
                // Lazy initialize seller wallet if it doesn't exist
                sellerWallet = new AuctionSys.Domain.Entities.Wallet { UserId = item.SellerId };
                sellerWallet.InitializeSignature(_signatureService);
                await _unitOfWork.Wallets.AddAsync(sellerWallet);
                // Cần Save để lấy ID nếu identity generated (Guid thường được sinh trước, DB sẽ nhận)
            }

            // 2. Trừ/Cộng tiền bằng ProcessTransaction
            buyerWallet.ProcessTransaction(item.Price, TransactionType.Purchase, $"Mua sản phẩm '{item.Title}' (-{item.Price:N0} VNĐ)", _signatureService);
            sellerWallet.ProcessTransaction(item.Price, TransactionType.Sale, $"Bán sản phẩm '{item.Title}' (+{item.Price:N0} VNĐ)", _signatureService);

            // 4. Update Item Status & Create Order
            item.Status = ItemStatus.Sold;
            await _unitOfWork.Items.UpdateAsync(item);

            var order = new AuctionSys.Domain.Entities.Order
            {
                BuyerId = buyerId,
                ItemId = itemId,
                TotalPrice = item.Price,
                Status = OrderStatus.Completed
            };
            await _unitOfWork.Orders.AddAsync(order);

            // 5. Notification
            var notification = new AuctionSys.Domain.Entities.Notification
            {
                UserId = item.SellerId,
                Type = NotificationType.OrderCompleted,
                Title = "Sản phẩm đã được bán!",
                Message = $"Sản phẩm '{item.Title}' của bạn đã được mua đứt với giá {item.Price:N0} VNĐ.",
                ReferenceId = itemId.ToString(),
                IsRead = false
            };
            await _unitOfWork.Notifications.AddAsync(notification);

            // 6. Commit DB Transaction
            await _unitOfWork.CommitTransactionAsync();
            await _unitOfWork.CompleteAsync();

            return ApiResponse<string>.Success("Mua sản phẩm thành công!");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw new Exception($"Purchase item error: {ex.Message}"); // Let Global Exception Middleware handle logging and returning 500
        }
        finally
        {
            // 7. Unlock Redis
            await db.LockReleaseAsync(lockKey, lockToken);
        }
    }
}
