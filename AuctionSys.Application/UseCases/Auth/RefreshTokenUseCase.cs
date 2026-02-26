using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Auth;
using AuctionSys.Application.Interfaces.Auth;
using AuctionSys.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.IdentityModel.Tokens.Jwt;

namespace AuctionSys.Application.UseCases.Auth;

public class RefreshTokenUseCase : AuctionSys.Application.Interfaces.UseCases.Auth.IRefreshTokenUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IDistributedCache _cache;

    public RefreshTokenUseCase(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator, IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
        _cache = cache;
    }

    public async Task<ApiResponse<AuthResponse>> ExecuteAsync(string accessToken, string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken))
            return ApiResponse<AuthResponse>.Fail("Token không hợp lệ", 401);

        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(accessToken))
            return ApiResponse<AuthResponse>.Fail("Access Token không định dạng đúng", 401);

        var jwtToken = handler.ReadJwtToken(accessToken);
        var userIdStr = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            return ApiResponse<AuthResponse>.Fail("Access Token không hợp lệ", 401);

        var savedRefreshToken = await _cache.GetStringAsync($"RT_{userId}");
        if (savedRefreshToken != refreshToken)
            return ApiResponse<AuthResponse>.Fail("Refresh Token không hợp lệ hoặc đã hết hạn", 401);

        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            return ApiResponse<AuthResponse>.Fail("Không tìm thấy người dùng", 404);

        var newToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
        };
        await _cache.SetStringAsync($"RT_{user.Id}", newRefreshToken, cacheOptions);

        return ApiResponse<AuthResponse>.Success(new AuthResponse
        {
            AccessToken = newToken,
            RefreshToken = newRefreshToken,
            Email = user.Email,
            FullName = user.FullName
        }, "Làm mới token thành công");
    }
}
