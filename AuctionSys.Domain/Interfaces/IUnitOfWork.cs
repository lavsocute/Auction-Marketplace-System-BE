namespace AuctionSys.Domain.Interfaces;

public interface IUnitOfWork : IAsyncDisposable, IDisposable
{
    IUserRepository Users { get; }
    IItemRepository Items { get; }
    IWalletRepository Wallets { get; }
    IWalletTransactionRepository WalletTransactions { get; }
    IAuctionRepository Auctions { get; }
    IBidRepository Bids { get; }
    IOrderRepository Orders { get; }
    ICategoryRepository Categories { get; }
    IChatMessageRepository ChatMessages { get; }
    IReviewRepository Reviews { get; }
    IWishlistRepository Wishlists { get; }
    INotificationRepository Notifications { get; }
    IReportRepository Reports { get; }
    IAuctionWatcherRepository AuctionWatchers { get; }
    
    
    // CS2 Domain
    IBotRepository Bots { get; }
    IBotInventoryRepository BotInventories { get; }

    Task<int> CompleteAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
