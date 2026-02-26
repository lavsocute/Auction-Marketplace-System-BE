using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuctionSys.Application.DTOs.Review;
using AuctionSys.Application.Interfaces.UseCases.Review;

namespace AuctionSys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly ICreateReviewUseCase _createReviewUseCase;

    public ReviewsController(ICreateReviewUseCase createReviewUseCase)
    {
        _createReviewUseCase = createReviewUseCase;
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new UnauthorizedAccessException("Không tìm thấy thông tin UserId trong token.");
        return Guid.Parse(userIdStr);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto request)
    {
        var response = await _createReviewUseCase.ExecuteAsync(GetUserId(), request);
        return StatusCode(response.StatusCode, response);
    }
}
