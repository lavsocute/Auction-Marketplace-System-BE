using System.ComponentModel.DataAnnotations;

namespace AuctionSys.Application.DTOs.Auction;

public class CreateAuctionDto
{
    [Required]
    public Guid ItemId { get; set; }

    [Required]
    [Range(0, 1000000000)]
    public decimal StartPrice { get; set; }

    [Required]
    public DateTime EndTime { get; set; }
}
