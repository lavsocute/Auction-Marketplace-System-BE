namespace AuctionSys.Application.DTOs.Wallet;

public class WalletDto
{
    public Guid Id { get; set; }
    public decimal Balance { get; set; }
    public decimal FrozenBalance { get; set; }
    public Guid UserId { get; set; }
}
