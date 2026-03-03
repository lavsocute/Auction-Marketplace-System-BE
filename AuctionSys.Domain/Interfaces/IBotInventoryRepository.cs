using AuctionSys.Domain.Entities;

namespace AuctionSys.Domain.Interfaces;

public interface IBotInventoryRepository : IAsyncRepository<BotInventory>
{
    // Need a special method to include Item for updating status
    Task<IReadOnlyList<BotInventory>> GetExpiredTradeLocksWithItemsAsync(DateTime currentTime);
}
