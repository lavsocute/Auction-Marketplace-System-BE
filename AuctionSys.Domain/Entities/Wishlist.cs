namespace AuctionSys.Domain.Entities;

public class Wishlist
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
