using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuctionSys.Application.Interfaces.UseCases.Wishlist;

namespace AuctionSys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistsController : ControllerBase
{
    private readonly IGetWishlistUseCase _getWishlistUseCase;
    private readonly IAddToWishlistUseCase _addToWishlistUseCase;
    private readonly IRemoveFromWishlistUseCase _removeFromWishlistUseCase;

    public WishlistsController(
        IGetWishlistUseCase getWishlistUseCase,
        IAddToWishlistUseCase addToWishlistUseCase,
        IRemoveFromWishlistUseCase removeFromWishlistUseCase)
    {
        _getWishlistUseCase = getWishlistUseCase;
        _addToWishlistUseCase = addToWishlistUseCase;
        _removeFromWishlistUseCase = removeFromWishlistUseCase;
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new UnauthorizedAccessException("Không tìm thấy thông tin UserId trong token.");
        return Guid.Parse(userIdStr);
    }

    [HttpGet]
    public async Task<IActionResult> GetWishlist()
    {
        var response = await _getWishlistUseCase.ExecuteAsync(GetUserId());
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("{itemId}")]
    public async Task<IActionResult> AddToWishlist(Guid itemId)
    {
        var response = await _addToWishlistUseCase.ExecuteAsync(GetUserId(), itemId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{itemId}")]
    public async Task<IActionResult> RemoveFromWishlist(Guid itemId)
    {
        var response = await _removeFromWishlistUseCase.ExecuteAsync(GetUserId(), itemId);
        return StatusCode(response.StatusCode, response);
    }
}
