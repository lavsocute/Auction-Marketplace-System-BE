using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Bot;
using AuctionSys.Application.Interfaces.UseCases.Bot;
using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuctionSys.Application.UseCases.Bot;

public class DepositItemUseCase : IDepositItemUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DepositItemUseCase> _logger;

    public DepositItemUseCase(IUnitOfWork unitOfWork, ILogger<DepositItemUseCase> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<string>> ExecuteAsync(Guid botId, Guid sellerId, DepositItemRequestDto request)
    {
        try
        {
            var bot = await _unitOfWork.Bots.GetByIdAsync(botId);
            if (bot == null || !bot.IsActive)
            {
                return ApiResponse<string>.Fail("Bot not found or missing from pool.", 404);
            }

            var user = await _unitOfWork.Users.GetByIdAsync(sellerId);
            if (user == null)
            {
                return ApiResponse<string>.Fail("User not found.", 404);
            }

            if (request.CategoryId.HasValue)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId.Value);
                if (category == null) return ApiResponse<string>.Fail("Invalid category ID.", 400);
            }

            // Create Item
            var item = new AuctionSys.Domain.Entities.Item
            {
                Title = request.Title,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                Price = request.Price,
                ListType = request.ListType,
                Status = ItemStatus.TradeLocked, // CS2 rule: Trade locked upon deposit
                SellerId = sellerId,
                CategoryId = request.CategoryId,
                AssetId = request.AssetId, // The ID from Steam inventory
                CreatedAt = DateTime.UtcNow
            };

            // Create Skin Metadata
            var skinMetadata = new SkinMetadata
            {
                ItemId = item.Id,
                Exterior = request.Exterior,
                FloatValue = request.FloatValue,
                PatternIndex = request.PatternIndex,
                IsStatTrak = request.IsStatTrak,
                NameTag = request.NameTag,
                StickersJson = request.StickersJson
            };

            item.SkinMetadata = skinMetadata;

            // Add to Bot Inventory with 7-day trade hold
            var botInventory = new BotInventory
            {
                BotId = bot.Id,
                ItemId = item.Id,
                TradeLockedUntil = DateTime.UtcNow.AddDays(7), // Set 7 day lock
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Items.AddAsync(item);
                // SkinMetadata is saved implicitly due to EF relationship mapping (Cascade)
                
                await _unitOfWork.BotInventories.AddAsync(botInventory);

                await _unitOfWork.CommitTransactionAsync();
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Item {item.Id} deposited into Bot {bot.Id} with 7-day trade lock.");
                return ApiResponse<string>.Success(item.Id.ToString(), "Item deposited successfully and is under trade lock.");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing deposit request");
            return ApiResponse<string>.Fail("An internal error occurred during deposit.", 500);
        }
    }
}
