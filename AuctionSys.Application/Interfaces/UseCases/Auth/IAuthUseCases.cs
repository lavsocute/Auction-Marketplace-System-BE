using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Auth;

namespace AuctionSys.Application.Interfaces.UseCases.Auth;

public interface ISendOtpUseCase
{
    Task<ApiResponse<string>> ExecuteAsync(SendOtpRequest request);
}

public interface IRegisterUseCase
{
    Task<ApiResponse<AuthResponse>> ExecuteAsync(RegisterRequest request);
}

public interface ILoginUseCase
{
    Task<ApiResponse<AuthResponse>> ExecuteAsync(LoginRequest request);
}

public interface IRefreshTokenUseCase
{
    Task<ApiResponse<AuthResponse>> ExecuteAsync(string accessToken, string refreshToken);
}

public interface ILogoutUseCase
{
    Task<ApiResponse<string>> ExecuteAsync(string accessToken, string refreshToken);
}
