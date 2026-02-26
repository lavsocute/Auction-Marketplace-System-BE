namespace AuctionSys.Application.Interfaces.Auth;

public interface ITokenBlacklistService
{
    Task BlacklistTokenAsync(string token, TimeSpan expiryTime);
    Task<bool> IsTokenBlacklistedAsync(string token);
}
