using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Interfaces;
using AuctionSys.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuctionSys.Infrastructure.Repositories;

public class UserRepository : AsyncRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbContext.Set<User>().AnyAsync(u => u.Email == email);
    }
}

public class WalletRepository : AsyncRepository<Wallet>, IWalletRepository
{
    public WalletRepository(AppDbContext context) : base(context) { }

    public async Task<Wallet?> GetByUserIdAsync(Guid userId)
    {
        return await _dbContext.Set<Wallet>().FirstOrDefaultAsync(w => w.UserId == userId);
    }
}

public class WalletTransactionRepository : AsyncRepository<WalletTransaction>, IWalletTransactionRepository
{
    public WalletTransactionRepository(AppDbContext context) : base(context) { }
}

public class AuctionRepository : AsyncRepository<Auction>, IAuctionRepository
{
    public AuctionRepository(AppDbContext context) : base(context) { }
}

public class BidRepository : AsyncRepository<Bid>, IBidRepository
{
    public BidRepository(AppDbContext context) : base(context) { }
}

public class OrderRepository : AsyncRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context) { }
}

public class CategoryRepository : AsyncRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }
}

public class ChatMessageRepository : AsyncRepository<ChatMessage>, IChatMessageRepository
{
    public ChatMessageRepository(AppDbContext context) : base(context) { }
}

public class ReviewRepository : AsyncRepository<Review>, IReviewRepository
{
    public ReviewRepository(AppDbContext context) : base(context) { }
}

public class WishlistRepository : AsyncRepository<Wishlist>, IWishlistRepository
{
    public WishlistRepository(AppDbContext context) : base(context) { }
}

public class NotificationRepository : AsyncRepository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context) { }
}

public class ReportRepository : AsyncRepository<Report>, IReportRepository
{
    public ReportRepository(AppDbContext context) : base(context) { }
}

public class AuctionWatcherRepository : AsyncRepository<AuctionWatcher>, IAuctionWatcherRepository
{
    public AuctionWatcherRepository(AppDbContext context) : base(context) { }
}
