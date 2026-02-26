using AuctionSys.Domain.Entities;

namespace AuctionSys.Application.Interfaces.Auth;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
