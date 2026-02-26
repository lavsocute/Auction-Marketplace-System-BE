using System.IdentityModel.Tokens.Jwt;
using AuctionSys.Application.Common.Models;
using AuctionSys.Application.Interfaces.Auth;
using Microsoft.Extensions.Caching.Distributed;

namespace AuctionSys.Application.UseCases.Auth;

public class LogoutUseCase : AuctionSys.Application.Interfaces.UseCases.Auth.ILogoutUseCase
{
    private readonly ITokenBlacklistService _blacklistService;
    private readonly IDistributedCache _cache;

    public LogoutUseCase(ITokenBlacklistService blacklistService, IDistributedCache cache)
    {
        _blacklistService = blacklistService;
        _cache = cache;
    }

    public async Task<ApiResponse<string>> ExecuteAsync(string accessToken, string refreshToken)
    {
        // 1. Blacklist Access Token
        if (!string.IsNullOrEmpty(accessToken))
        {
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(accessToken))
            {
                var jwtToken = handler.ReadJwtToken(accessToken);
                var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                
                // Lấy thời gian hết hạn còn lại
                var expiryTime = jwtToken.ValidTo - DateTime.UtcNow;
                if (expiryTime > TimeSpan.Zero)
                {
                    await _blacklistService.BlacklistTokenAsync(accessToken, expiryTime);
                }
                
                // Có thể xóa luôn RefreshToken từ cache nếu extract được userId
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    await _cache.RemoveAsync($"RT_{userId}");
                }
            }
        }

        return ApiResponse<string>.Success(string.Empty, "Đăng xuất thành công");
    }
}
