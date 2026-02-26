namespace AuctionSys.Domain.Entities;

public class Bid
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AuctionId { get; set; }
    public Auction Auction { get; set; } = null!;

    public Guid BidderId { get; set; }
    public User Bidder { get; set; } = null!;

    public decimal Amount { get; set; }
    public DateTime BidTime { get; set; } = DateTime.UtcNow;
}
