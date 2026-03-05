using AuctionSys.Domain.Entities;

namespace AuctionSys.Domain.Interfaces;

public interface IUserRepository : IAsyncRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
}

public interface IWalletRepository : IAsyncRepository<Wallet>
{
    Task<Wallet?> GetByUserIdAsync(Guid userId);
}

public interface IWalletTransactionRepository : IAsyncRepository<WalletTransaction>
{
}

public interface IAuctionRepository : IAsyncRepository<Auction>
{
}

public interface IBidRepository : IAsyncRepository<Bid>
{
}

public interface IOrderRepository : IAsyncRepository<Order>
{
}

public interface ICategoryRepository : IAsyncRepository<Category>
{
}

public interface IChatMessageRepository : IAsyncRepository<ChatMessage>
{
}

public interface IReviewRepository : IAsyncRepository<Review>
{
}

public interface IWishlistRepository : IAsyncRepository<Wishlist>
{
}

public interface INotificationRepository : IAsyncRepository<Notification>
{
}

public interface IReportRepository : IAsyncRepository<Report>
{
}

public interface IAuctionWatcherRepository : IAsyncRepository<AuctionWatcher>
{
}
