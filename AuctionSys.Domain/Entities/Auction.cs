using AuctionSys.Domain.Enums;

namespace AuctionSys.Domain.Entities;

public class Auction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public decimal StartPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime EndTime { get; set; }
    
    public AuctionStatus Status { get; set; } = AuctionStatus.Active;

    public Guid? WinnerId { get; set; }
    public User? Winner { get; set; }

    public ICollection<Bid> Bids { get; set; } = new List<Bid>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
