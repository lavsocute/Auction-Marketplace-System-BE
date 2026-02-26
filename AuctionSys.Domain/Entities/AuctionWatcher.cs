namespace AuctionSys.Domain.Entities;

public class AuctionWatcher
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid AuctionId { get; set; }
    public Auction Auction { get; set; } = null!;

    public DateTime WatchedAt { get; set; } = DateTime.UtcNow;
}
