using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Auth;
using AuctionSys.Application.Interfaces.Auth;
using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace AuctionSys.Application.UseCases.Auth;

public class RegisterUseCase : AuctionSys.Application.Interfaces.UseCases.Auth.IRegisterUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDistributedCache _cache;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterUseCase(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IDistributedCache cache,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _cache = cache;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ApiResponse<AuthResponse>> ExecuteAsync(RegisterRequest request)
    {
        // Kiểm tra OTP trong Redis
        var savedOtp = await _cache.GetStringAsync($"OTP_{request.Email}");
        if (string.IsNullOrEmpty(savedOtp) || savedOtp != request.OtpCode)
        {
            return ApiResponse<AuthResponse>.Fail("Mã OTP không hợp lệ hoặc đã hết hạn.");
        }

        // Kiểm tra email tồn tại (phòng trường hợp Register cùng lúc 2 người)
        if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
        {
            return ApiResponse<AuthResponse>.Fail("Email đã được sử dụng!");
        }

        var user = new AuctionSys.Domain.Entities.User
        {
            Email = request.Email,
            FullName = request.FullName,
            PasswordHash = _passwordHasher.HashPassword(request.Password)
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();

        // Xoá OTP đi để tránh dùng lại (Single Use)
        await _cache.RemoveAsync($"OTP_{request.Email}");

        var token = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        
        // Lưu Refresh Token vào Cache (7 ngày)
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
        };
        await _cache.SetStringAsync($"RT_{user.Id}", refreshToken, cacheOptions);
        
        return ApiResponse<AuthResponse>.Success(new AuthResponse
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            Email = user.Email,
            FullName = user.FullName
        }, "Đăng ký thành công!");
    }
}
