using AuctionSys.Application.Common;
using AuctionSys.Application.DTOs.Bot;
using AuctionSys.Application.Interfaces.UseCases.Bot;
using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuctionSys.Application.UseCases.Bot;

public class AddBotUseCase : IAddBotUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddBotUseCase> _logger;

    public AddBotUseCase(IUnitOfWork unitOfWork, ILogger<AddBotUseCase> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<BotResponseDto>> ExecuteAsync(CreateBotRequestDto request)
    {
        try
        {
            var existingBot = await _unitOfWork.Bots.GetBySteamIdAsync(request.SteamId);
            if (existingBot != null)
            {
                return ApiResponse<BotResponseDto>.Error("Bot with this Steam ID already exists.");
            }

            var bot = new AuctionSys.Domain.Entities.Bot
            {
                SteamId = request.SteamId,
                Name = request.Name,
                TradeUrl = request.TradeUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Bots.AddAsync(bot);
            await _unitOfWork.CompleteAsync();

            var response = new BotResponseDto
            {
                Id = bot.Id,
                SteamId = bot.SteamId,
                Name = bot.Name,
                TradeUrl = bot.TradeUrl,
                IsActive = bot.IsActive,
                CreatedAt = bot.CreatedAt
            };

            _logger.LogInformation($"Successfully added bot {bot.Name} with ID {bot.Id}");
            return ApiResponse<BotResponseDto>.Success(response, "Bot added successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while adding bot");
            return ApiResponse<BotResponseDto>.Error("An internal error occurred while adding the bot.");
        }
    }
}
