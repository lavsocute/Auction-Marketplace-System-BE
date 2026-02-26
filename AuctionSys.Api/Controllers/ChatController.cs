using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuctionSys.Application.DTOs.Chat;
using AuctionSys.Application.Interfaces.UseCases.Chat;

namespace AuctionSys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IGetConversationsUseCase _getConversationsUseCase;
    private readonly IGetChatHistoryUseCase _getChatHistoryUseCase;
    private readonly ISendMessageUseCase _sendMessageUseCase;
    
    // In a real application, sendMessage might be handled entirely by SignalR,
    // but having a REST fallback is also common practice.

    public ChatController(
        IGetConversationsUseCase getConversationsUseCase,
        IGetChatHistoryUseCase getChatHistoryUseCase,
        ISendMessageUseCase sendMessageUseCase)
    {
        _getConversationsUseCase = getConversationsUseCase;
        _getChatHistoryUseCase = getChatHistoryUseCase;
        _sendMessageUseCase = sendMessageUseCase;
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new UnauthorizedAccessException("Không tìm thấy thông tin UserId trong token.");
        return Guid.Parse(userIdStr);
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
    {
        var response = await _getConversationsUseCase.ExecuteAsync(GetUserId());
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{partnerId}")]
    public async Task<IActionResult> GetChatHistory(Guid partnerId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var response = await _getChatHistoryUseCase.ExecuteAsync(GetUserId(), partnerId, pageNumber, pageSize);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto request)
    {
        var response = await _sendMessageUseCase.ExecuteAsync(GetUserId(), request);
        return StatusCode(response.StatusCode, response);
    }
}
