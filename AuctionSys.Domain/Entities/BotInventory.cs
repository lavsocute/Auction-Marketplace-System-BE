using AuctionSys.Domain.Enums;

namespace AuctionSys.Domain.Entities;

public class BotInventory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid BotId { get; set; }
    public Bot Bot { get; set; } = null!;
    
    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;
    
    public DateTime? TradeLockedUntil { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
