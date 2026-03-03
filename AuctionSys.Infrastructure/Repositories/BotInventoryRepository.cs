using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;
using AuctionSys.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuctionSys.Infrastructure.Repositories;

public class BotInventoryRepository : AsyncRepository<BotInventory>, IBotInventoryRepository
{
    public BotInventoryRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<BotInventory>> GetExpiredTradeLocksWithItemsAsync(DateTime currentTime)
    {
        return await _dbContext.BotInventories
            .Include(bi => bi.Item)
            .Where(bi => bi.Item.Status == ItemStatus.TradeLocked &&
                         bi.TradeLockedUntil != null &&
                         bi.TradeLockedUntil <= currentTime)
            .ToListAsync();
    }
}
