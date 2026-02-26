using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using AuctionSys.Application.DTOs.Chat;

namespace AuctionSys.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    // Cần DI ISendMessageUseCase vào đây nếu muốn client gọi trực tiếp qua WebSocket
    // Hoặc client có thể gọi POST /api/chat, sau đó backend tự push vào Hub thông qua IHubContext<ChatHub>
    // Trong demo này, tôi sẽ hướng dẫn client gọi POST /api/chat. Nếu muốn xử lý SendMessage ngay trong Hub, hãy inject UseCase.
    
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Client gửi method này từ JS: connection.invoke("SendMessage", messageDto)
    // Hoặc dùng REST API. Dưới đây là ví dụ setup để HubContext được gọi từ UseCase.
}
