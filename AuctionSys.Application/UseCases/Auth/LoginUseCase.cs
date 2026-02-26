using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Auth;
using AuctionSys.Application.Interfaces.Auth;
using AuctionSys.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace AuctionSys.Application.UseCases.Auth;

public class LoginUseCase : AuctionSys.Application.Interfaces.UseCases.Auth.ILoginUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IDistributedCache _cache;

    public LoginUseCase(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _cache = cache;
    }

    public async Task<ApiResponse<AuthResponse>> ExecuteAsync(LoginRequest request)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return ApiResponse<AuthResponse>.Fail("Email hoặc mật khẩu không chính xác!", 401);
        }

        var token = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        // Lưu Refresh Token vào Cache (7 ngày)
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
        };
        // Lưu theo UserID để quản lý
        await _cache.SetStringAsync($"RT_{user.Id}", refreshToken, cacheOptions);

        return ApiResponse<AuthResponse>.Success(new AuthResponse
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            Email = user.Email,
            FullName = user.FullName
        }, "Đăng nhập thành công!");
    }
}
