using AuctionSys.Domain.Enums;

namespace AuctionSys.Domain.Entities;

public class Bot
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SteamId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? TradeUrl { get; set; }
    public bool IsActive { get; set; } = true;
    
    public ICollection<BotInventory> Inventories { get; set; } = new List<BotInventory>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
