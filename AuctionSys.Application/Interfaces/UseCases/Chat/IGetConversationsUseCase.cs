using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Chat;

namespace AuctionSys.Application.Interfaces.UseCases.Chat;

public interface IGetConversationsUseCase
{
    Task<ApiResponse<List<ConversationDto>>> ExecuteAsync(Guid userId);
}
