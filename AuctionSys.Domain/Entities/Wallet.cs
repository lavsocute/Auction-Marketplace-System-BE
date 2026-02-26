namespace AuctionSys.Domain.Entities;

public class Wallet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Balance { get; set; } = 0;
    public decimal FrozenBalance { get; set; } = 0;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
