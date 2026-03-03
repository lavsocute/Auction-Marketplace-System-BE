using AuctionSys.Application.Common;
using AuctionSys.Application.DTOs.Bot;
using AuctionSys.Application.Interfaces.UseCases.Bot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuctionSys.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BotsController : ControllerBase
{
    private readonly IAddBotUseCase _addBotUseCase;
    private readonly IDepositItemUseCase _depositItemUseCase;
    private readonly IWithdrawItemUseCase _withdrawItemUseCase;

    public BotsController(
        IAddBotUseCase addBotUseCase,
        IDepositItemUseCase depositItemUseCase,
        IWithdrawItemUseCase withdrawItemUseCase)
    {
        _addBotUseCase = addBotUseCase;
        _depositItemUseCase = depositItemUseCase;
        _withdrawItemUseCase = withdrawItemUseCase;
    }

    [HttpPost]
    [Authorize] // Ideally restrict to Admins
    public async Task<IActionResult> AddBot([FromBody] CreateBotRequestDto request)
    {
        var response = await _addBotUseCase.ExecuteAsync(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("{botId:guid}/deposit")]
    [Authorize]
    public async Task<IActionResult> DepositItem(Guid botId, [FromBody] DepositItemRequestDto request)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized(ApiResponse<string>.Error("Invalid token or user not found."));

        var response = await _depositItemUseCase.ExecuteAsync(botId, userId, request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("{botId:guid}/items/{itemId:guid}/withdraw")]
    [Authorize]
    public async Task<IActionResult> WithdrawItem(Guid botId, Guid itemId)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized(ApiResponse<bool>.Error("Invalid token or user not found."));

        var response = await _withdrawItemUseCase.ExecuteAsync(botId, itemId, userId);
        return StatusCode(response.StatusCode, response);
    }
}
