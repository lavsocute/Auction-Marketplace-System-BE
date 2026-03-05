using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Bot;

namespace AuctionSys.Application.Interfaces.UseCases.Bot;

public interface IAddBotUseCase
{
    Task<ApiResponse<BotResponseDto>> ExecuteAsync(CreateBotRequestDto request);
}
