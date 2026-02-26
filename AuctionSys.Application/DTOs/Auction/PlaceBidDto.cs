using System.ComponentModel.DataAnnotations;

namespace AuctionSys.Application.DTOs.Auction;

public class PlaceBidDto
{
    [Required]
    [Range(1000, 1000000000)]
    public decimal Amount { get; set; }
}
