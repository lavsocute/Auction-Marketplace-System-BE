using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Chat;

namespace AuctionSys.Application.Interfaces.UseCases.Chat;

public interface IGetChatHistoryUseCase
{
    Task<ApiResponse<PagedResponse<ChatMessageDto>>> ExecuteAsync(Guid userId, Guid partnerId, int pageNumber = 1, int pageSize = 20);
}
