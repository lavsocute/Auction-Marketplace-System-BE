using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Auth;
using AuctionSys.Application.Interfaces.Auth;
using AuctionSys.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace AuctionSys.Application.UseCases.Auth;

public class SendOtpUseCase : AuctionSys.Application.Interfaces.UseCases.Auth.ISendOtpUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IDistributedCache _cache;

    public SendOtpUseCase(IUnitOfWork unitOfWork, IEmailService emailService, IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _cache = cache;
    }

    public async Task<ApiResponse<string>> ExecuteAsync(SendOtpRequest request)
    {
        if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
            return ApiResponse<string>.Fail("Email đã được sử dụng!");

        // Sinh OTP ngẫu nhiên 6 số
        var otp = new Random().Next(100000, 999999).ToString();

        // Lưu vào Redis (TTL 2 phút)
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
        };
        await _cache.SetStringAsync($"OTP_{request.Email}", otp, cacheOptions);

        await _emailService.SendOtpEmailAsync(request.Email, otp);

        return ApiResponse<string>.Success(string.Empty, "Đã gửi mã OTP đến email của bạn. Mã có hiệu lực trong 2 phút.");
    }
}
