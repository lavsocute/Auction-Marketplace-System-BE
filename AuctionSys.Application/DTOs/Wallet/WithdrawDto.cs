using System.ComponentModel.DataAnnotations;

namespace AuctionSys.Application.DTOs.Wallet;

public class WithdrawDto
{
    [Required]
    [Range(10000, 100000000)]
    public decimal Amount { get; set; }
    
    [Required]
    public string BankAccountNumber { get; set; } = string.Empty;
    
    [Required]
    public string BankName { get; set; } = string.Empty;
}
