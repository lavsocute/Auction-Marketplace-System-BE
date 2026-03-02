using AuctionSys.Domain.Enums;

namespace AuctionSys.Domain.Entities;

public class Item
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public ListType ListType { get; set; }
    public ItemStatus Status { get; set; } = ItemStatus.InUserSteamInventory;
    
    public string? AssetId { get; set; } // Steam Asset ID

    public Guid SellerId { get; set; }
    public User Seller { get; set; } = null!;

    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    public Auction? Auction { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    
    public SkinMetadata? SkinMetadata { get; set; }
    public ICollection<BotInventory> BotInventories { get; set; } = new List<BotInventory>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
