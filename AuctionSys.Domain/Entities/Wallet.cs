using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Domain.Entities;

public class Wallet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Balance { get; private set; } = 0;
    public decimal FrozenBalance { get; private set; } = 0;
    public string Signature { get; private set; } = string.Empty;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Constructs a new wallet. Signature should be initialized immediately.
    /// </summary>
    public void InitializeSignature(IWalletSignatureService signatureService)
    {
        Signature = signatureService.GenerateSignature(Id, UserId, Balance, FrozenBalance);
    }

    /// <summary>
    /// Validates the data integrity of the wallet. Throws if tampered.
    /// </summary>
    public void ValidateSignature(IWalletSignatureService signatureService)
    {
        if (string.IsNullOrEmpty(Signature))
        {
            // For backward compatibility or first time initialization
            return;
        }

        if (!signatureService.VerifySignature(Id, UserId, Balance, FrozenBalance, Signature))
        {
            throw new Exception("Financial Security Alert: Wallet integrity check failed. Balance tampering detected.");
        }
    }

    /// <summary>
    /// Core method for Append-Only Ledger. Processes a transaction and updates balance securely.
    /// </summary>
    public void ProcessTransaction(decimal amount, TransactionType type, string description, IWalletSignatureService signatureService)
    {
        // 1. Verify current state hasn't been tampered with
        ValidateSignature(signatureService);

        // 2. Apply business logic based on transaction type
        switch (type)
        {
            case TransactionType.TopUp:
            case TransactionType.Sale:
            case TransactionType.AuctionWin:
            case TransactionType.AuctionRefund:
            case TransactionType.BidUnfreeze:
                Balance += amount;
                break;
            case TransactionType.Withdraw:
            case TransactionType.Purchase:
                if (Balance < amount) throw new Exception("Insufficient balance.");
                Balance -= amount;
                break;
            case TransactionType.BidFreeze:
                if (Balance < amount) throw new Exception("Insufficient balance to freeze.");
                Balance -= amount;
                FrozenBalance += amount;
                break;
        }

        // Special handling if bid is unfreezing
        if (type == TransactionType.BidUnfreeze)
        {
            if (FrozenBalance < amount) throw new Exception("Insufficient frozen balance to unfreeze.");
            FrozenBalance -= amount;
        }

        // 3. Create transaction record (Append-Only Ledger)
        var transaction = new WalletTransaction
        {
            WalletId = Id,
            Amount = amount,
            Type = type,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };
        Transactions.Add(transaction);

        // 4. Update secure signature with new balances
        Signature = signatureService.GenerateSignature(Id, UserId, Balance, FrozenBalance);
        UpdatedAt = DateTime.UtcNow;
    }
}
