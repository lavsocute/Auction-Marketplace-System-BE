using AuctionSys.Application.DTOs.Item;
using AuctionSys.Application.DTOs.User;
using AuctionSys.Domain.Enums;

namespace AuctionSys.Application.DTOs.Auction;

public class AuctionDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public ItemDto Item { get; set; } = null!;
    
    public decimal StartPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    public AuctionStatus Status { get; set; }

    public Guid? WinnerId { get; set; }
    public UserProfileDto? Winner { get; set; }

    public DateTime CreatedAt { get; set; }
}
