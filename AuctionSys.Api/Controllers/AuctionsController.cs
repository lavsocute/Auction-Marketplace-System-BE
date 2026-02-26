using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuctionSys.Application.DTOs.Auction;
using AuctionSys.Application.Interfaces.UseCases.Auction;

namespace AuctionSys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController : ControllerBase
{
    private readonly IGetAuctionsUseCase _getAuctionsUseCase;
    private readonly ICreateAuctionUseCase _createAuctionUseCase;
    private readonly IGetAuctionDetailUseCase _getAuctionDetailUseCase;
    private readonly IPlaceBidUseCase _placeBidUseCase;
    private readonly IWatchAuctionUseCase _watchAuctionUseCase;
    private readonly IUnwatchAuctionUseCase _unwatchAuctionUseCase;

    public AuctionsController(
        IGetAuctionsUseCase getAuctionsUseCase,
        ICreateAuctionUseCase createAuctionUseCase,
        IGetAuctionDetailUseCase getAuctionDetailUseCase,
        IPlaceBidUseCase placeBidUseCase,
        IWatchAuctionUseCase watchAuctionUseCase,
        IUnwatchAuctionUseCase unwatchAuctionUseCase)
    {
        _getAuctionsUseCase = getAuctionsUseCase;
        _createAuctionUseCase = createAuctionUseCase;
        _getAuctionDetailUseCase = getAuctionDetailUseCase;
        _placeBidUseCase = placeBidUseCase;
        _watchAuctionUseCase = watchAuctionUseCase;
        _unwatchAuctionUseCase = unwatchAuctionUseCase;
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new UnauthorizedAccessException("Không tìm thấy thông tin UserId trong token.");
        return Guid.Parse(userIdStr);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAuctions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? status = null)
    {
        var response = await _getAuctionsUseCase.ExecuteAsync(pageNumber, pageSize, status);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAuctionDetail(Guid id)
    {
        var response = await _getAuctionDetailUseCase.ExecuteAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateAuction([FromBody] CreateAuctionDto request)
    {
        var response = await _createAuctionUseCase.ExecuteAsync(GetUserId(), request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("{id}/bid")]
    [Authorize]
    public async Task<IActionResult> PlaceBid(Guid id, [FromBody] PlaceBidDto request)
    {
        var response = await _placeBidUseCase.ExecuteAsync(GetUserId(), id, request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("{id}/watch")]
    [Authorize]
    public async Task<IActionResult> WatchAuction(Guid id)
    {
        var response = await _watchAuctionUseCase.ExecuteAsync(GetUserId(), id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id}/watch")]
    [Authorize]
    public async Task<IActionResult> UnwatchAuction(Guid id)
    {
        var response = await _unwatchAuctionUseCase.ExecuteAsync(GetUserId(), id);
        return StatusCode(response.StatusCode, response);
    }
}
