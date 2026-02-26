using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuctionSys.Application.DTOs.User;
using AuctionSys.Application.Interfaces.UseCases.User;
using AuctionSys.Application.Interfaces.UseCases.Review;

namespace AuctionSys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IGetProfileUseCase _getProfileUseCase;
    private readonly IUpdateProfileUseCase _updateProfileUseCase;
    private readonly IGetUserReviewsUseCase _getUserReviewsUseCase;

    public UsersController(
        IGetProfileUseCase getProfileUseCase,
        IUpdateProfileUseCase updateProfileUseCase,
        IGetUserReviewsUseCase getUserReviewsUseCase)
    {
        _getProfileUseCase = getProfileUseCase;
        _updateProfileUseCase = updateProfileUseCase;
        _getUserReviewsUseCase = getUserReviewsUseCase;
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new UnauthorizedAccessException("Không tìm thấy thông tin UserId trong token.");
        return Guid.Parse(userIdStr);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProfile(Guid id)
    {
        var response = await _getProfileUseCase.ExecuteAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto request)
    {
        var response = await _updateProfileUseCase.ExecuteAsync(GetUserId(), request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id}/reviews")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUserReviews(Guid id)
    {
        var response = await _getUserReviewsUseCase.ExecuteAsync(id);
        return StatusCode(response.StatusCode, response);
    }
}
