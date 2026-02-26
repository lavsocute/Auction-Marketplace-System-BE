using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Chat;

namespace AuctionSys.Application.Interfaces.UseCases.Chat;

public interface ISendMessageUseCase
{
    Task<ApiResponse<ChatMessageDto>> ExecuteAsync(Guid senderId, SendMessageDto request);
}
