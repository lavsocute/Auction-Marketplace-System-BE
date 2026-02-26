using AuctionSys.Application.Interfaces.Auth;
using Microsoft.Extensions.Caching.Distributed;

namespace AuctionSys.Infrastructure.Services.Auth;

public class TokenBlacklistService : ITokenBlacklistService
{
    private readonly IDistributedCache _cache;

    public TokenBlacklistService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task BlacklistTokenAsync(string token, TimeSpan expiryTime)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiryTime
        };
        await _cache.SetStringAsync($"BL_{token}", "revoked", options);
    }

    public async Task<bool> IsTokenBlacklistedAsync(string token)
    {
        var val = await _cache.GetStringAsync($"BL_{token}");
        return !string.IsNullOrEmpty(val);
    }
}
