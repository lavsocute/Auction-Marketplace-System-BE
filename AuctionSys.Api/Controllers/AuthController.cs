using AuctionSys.Application.DTOs.Auth;
using AuctionSys.Application.Interfaces.UseCases.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionSys.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ISendOtpUseCase _sendOtpUseCase;
    private readonly IRegisterUseCase _registerUseCase;
    private readonly ILoginUseCase _loginUseCase;
    private readonly IRefreshTokenUseCase _refreshTokenUseCase;
    private readonly ILogoutUseCase _logoutUseCase;

    public AuthController(
        ISendOtpUseCase sendOtpUseCase,
        IRegisterUseCase registerUseCase,
        ILoginUseCase loginUseCase,
        IRefreshTokenUseCase refreshTokenUseCase,
        ILogoutUseCase logoutUseCase)
    {
        _sendOtpUseCase = sendOtpUseCase;
        _registerUseCase = registerUseCase;
        _loginUseCase = loginUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
        _logoutUseCase = logoutUseCase;
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
    {
        var response = await _sendOtpUseCase.ExecuteAsync(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _registerUseCase.ExecuteAsync(request);
        if (response.IsSuccess && response.Data != null)
        {
            SetRefreshTokenCookie(response.Data.RefreshToken);
        }
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _loginUseCase.ExecuteAsync(request);
        if (response.IsSuccess && response.Data != null)
        {
            SetRefreshTokenCookie(response.Data.RefreshToken);
        }
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized(new { message = "Thiếu token để làm mới" });
        }

        var response = await _refreshTokenUseCase.ExecuteAsync(accessToken, refreshToken);
        if (response.IsSuccess && response.Data != null)
        {
            SetRefreshTokenCookie(response.Data.RefreshToken);
        }
        return StatusCode(response.StatusCode, response);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        var response = await _logoutUseCase.ExecuteAsync(accessToken, refreshToken ?? string.Empty);
        
        Response.Cookies.Delete("refreshToken");
        return StatusCode(response.StatusCode, response);
    }

    private void SetRefreshTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7),
            Secure = true,
            SameSite = SameSiteMode.Strict
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }
}
