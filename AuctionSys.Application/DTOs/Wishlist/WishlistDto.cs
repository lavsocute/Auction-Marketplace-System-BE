using AuctionSys.Application.DTOs.Item;

namespace AuctionSys.Application.DTOs.Wishlist;

public class WishlistDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public ItemDto Item { get; set; } = null!;
    public DateTime AddedAt { get; set; }
}
