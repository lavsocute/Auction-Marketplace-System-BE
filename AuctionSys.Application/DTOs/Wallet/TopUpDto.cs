using System.ComponentModel.DataAnnotations;

namespace AuctionSys.Application.DTOs.Wallet;

public class TopUpDto
{
    [Required]
    [Range(10000, 100000000)]
    public decimal Amount { get; set; }
}
