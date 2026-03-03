using AuctionSys.Application.Common.Models;
using AuctionSys.Application.Interfaces.UseCases.Bot;
using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuctionSys.Application.UseCases.Bot;

public class WithdrawItemUseCase : IWithdrawItemUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WithdrawItemUseCase> _logger;

    public WithdrawItemUseCase(IUnitOfWork unitOfWork, ILogger<WithdrawItemUseCase> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<bool>> ExecuteAsync(Guid botId, Guid itemId, Guid userId)
    {
        try
        {
            var item = await _unitOfWork.Items.GetByIdAsync(itemId);
            if (item == null) return ApiResponse<bool>.Error("Item not found.");
            
            // Validate Ownership
            if (item.SellerId != userId)
            {
                return ApiResponse<bool>.Error("You cannot withdraw an item you do not own.");
            }

            // Check Trade Lock Status (must be purely InBotInventory, not TradeLocked)
            if (item.Status == ItemStatus.TradeLocked)
            {
                return ApiResponse<bool>.Error("This item is currently trade-locked and cannot be withdrawn.");
            }

            if (item.Status != ItemStatus.InBotInventory)
            {
                return ApiResponse<bool>.Error($"This item cannot be withdrawn because its true status is {item.Status}.");
            }

            // Find matching Bot Inventory record to ensure its valid
            var inventoryRecords = await _unitOfWork.BotInventories.GetAsync(bi => bi.ItemId == itemId && bi.BotId == botId);
            var inventoryRecord = inventoryRecords.FirstOrDefault();
            
            if (inventoryRecord == null)
            {
                return ApiResponse<bool>.Error("This item is not present on the specified bot.");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Update item status back to Steam Inventory representation
                item.Status = ItemStatus.Withdrawn;
                item.UpdatedAt = DateTime.UtcNow;
                
                await _unitOfWork.Items.UpdateAsync(item);
                
                // Usually we'd delete the `BotInventory` mapping or update it as withdrawn history, lets delete for simplicity.
                await _unitOfWork.BotInventories.DeleteAsync(inventoryRecord);

                await _unitOfWork.CommitTransactionAsync();
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Item {item.Id} withdrawn from Bot {botId} back to Steam.");
                return ApiResponse<bool>.Success(true, "Item successfully withdrawn to Steam Inventory.");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing withdraw request");
            return ApiResponse<bool>.Error("An internal error occurred during the withdrawal.");
        }
    }
}
