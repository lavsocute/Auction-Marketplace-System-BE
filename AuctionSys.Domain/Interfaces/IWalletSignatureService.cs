using System;

namespace AuctionSys.Domain.Interfaces;

public interface IWalletSignatureService
{
    string GenerateSignature(Guid walletId, Guid userId, decimal balance, decimal frozenBalance);
    bool VerifySignature(Guid walletId, Guid userId, decimal balance, decimal frozenBalance, string expectedSignature);
}
