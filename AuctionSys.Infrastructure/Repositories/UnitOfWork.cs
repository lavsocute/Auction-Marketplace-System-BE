using AuctionSys.Domain.Interfaces;
using AuctionSys.Infrastructure.Data;

namespace AuctionSys.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;

    public IUserRepository Users { get; }
    public IItemRepository Items { get; }
    public IWalletRepository Wallets { get; }
    public IWalletTransactionRepository WalletTransactions { get; }
    public IAuctionRepository Auctions { get; }
    public IBidRepository Bids { get; }
    public IOrderRepository Orders { get; }
    public ICategoryRepository Categories { get; }
    public IChatMessageRepository ChatMessages { get; }
    public IReviewRepository Reviews { get; }
    public IWishlistRepository Wishlists { get; }
    public INotificationRepository Notifications { get; }
    public IReportRepository Reports { get; }
    public IAuctionWatcherRepository AuctionWatchers { get; }
    public IBotInventoryRepository BotInventories { get; }

    public UnitOfWork(
        AppDbContext context,
        IUserRepository users,
        IItemRepository items,
        IWalletRepository wallets,
        IWalletTransactionRepository walletTransactions,
        IAuctionRepository auctions,
        IBidRepository bids,
        IOrderRepository orders,
        ICategoryRepository categories,
        IChatMessageRepository chatMessages,
        IReviewRepository reviews,
        IWishlistRepository wishlists,
        INotificationRepository notifications,
        IReportRepository reports,
        IAuctionWatcherRepository auctionWatchers,
        IBotInventoryRepository botInventories)
    {
        _context = context;
        Users = users;
        Items = items;
        Wallets = wallets;
        WalletTransactions = walletTransactions;
        Auctions = auctions;
        Bids = bids;
        Orders = orders;
        Categories = categories;
        ChatMessages = chatMessages;
        Reviews = reviews;
        Wishlists = wishlists;
        Notifications = notifications;
        Reports = reports;
        AuctionWatchers = auctionWatchers;
        BotInventories = botInventories;
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
