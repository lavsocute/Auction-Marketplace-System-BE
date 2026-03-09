using System;
using System.Security.Cryptography;
using System.Text;
using AuctionSys.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AuctionSys.Infrastructure.Security;

public class WalletSignatureService : IWalletSignatureService
{
    private readonly string _secretKey;

    public WalletSignatureService(IConfiguration configuration)
    {
        _secretKey = configuration["Jwt:Key"] ?? "default_secret_key_for_dev_mode_only_1234567890";
    }

    public string GenerateSignature(Guid walletId, Guid userId, decimal balance, decimal frozenBalance)
    {
        var payload = $"{walletId}_{userId}_{balance:F2}_{frozenBalance:F2}_{_secretKey}";
        return ComputeHmacSha256(payload, _secretKey);
    }

    public bool VerifySignature(Guid walletId, Guid userId, decimal balance, decimal frozenBalance, string expectedSignature)
    {
        var actualSignature = GenerateSignature(walletId, userId, balance, frozenBalance);
        return actualSignature == expectedSignature;
    }

    private string ComputeHmacSha256(string payload, string secretKey)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(payloadBytes);
        return Convert.ToBase64String(hashBytes);
    }
}
