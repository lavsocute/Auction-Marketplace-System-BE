using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuctionSys.Application.Interfaces.UseCases.Notification;

namespace AuctionSys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IGetNotificationsUseCase _getNotificationsUseCase;
    private readonly IMarkAsReadUseCase _markAsReadUseCase;
    private readonly IMarkAllAsReadUseCase _markAllAsReadUseCase;

    public NotificationsController(
        IGetNotificationsUseCase getNotificationsUseCase,
        IMarkAsReadUseCase markAsReadUseCase,
        IMarkAllAsReadUseCase markAllAsReadUseCase)
    {
        _getNotificationsUseCase = getNotificationsUseCase;
        _markAsReadUseCase = markAsReadUseCase;
        _markAllAsReadUseCase = markAllAsReadUseCase;
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new UnauthorizedAccessException("Không tìm thấy thông tin UserId trong token.");
        return Guid.Parse(userIdStr);
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _getNotificationsUseCase.ExecuteAsync(GetUserId(), pageNumber, pageSize);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var response = await _markAsReadUseCase.ExecuteAsync(GetUserId(), id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var response = await _markAllAsReadUseCase.ExecuteAsync(GetUserId());
        return StatusCode(response.StatusCode, response);
    }
}
