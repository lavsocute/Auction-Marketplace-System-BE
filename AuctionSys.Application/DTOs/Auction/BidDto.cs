using AuctionSys.Application.DTOs.User;
using AuctionSys.Domain.Enums;

namespace AuctionSys.Application.DTOs.Auction;

public class BidDto
{
    public Guid Id { get; set; }
    public Guid AuctionId { get; set; }
    public Guid BidderId { get; set; }
    public UserProfileDto Bidder { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime BidTime { get; set; }
}
