using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuctionSys.Application.UseCases.Bot;

public class ReleaseTradeLockUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReleaseTradeLockUseCase> _logger;

    public ReleaseTradeLockUseCase(IUnitOfWork unitOfWork, ILogger<ReleaseTradeLockUseCase> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting Trade Lock Release job...");

        try
        {
            var now = DateTime.UtcNow;

            var expiredLocks = await _unitOfWork.BotInventories.GetExpiredTradeLocksWithItemsAsync(now);

            if (!expiredLocks.Any())
            {
                _logger.LogInformation("No expired trade locks found.");
                return;
            }

            _logger.LogInformation($"Found {expiredLocks.Count} items to release trade lock.");

            foreach (var botInventory in expiredLocks)
            {
                botInventory.Item.Status = ItemStatus.InBotInventory;
                botInventory.Item.UpdatedAt = now;
                
                await _unitOfWork.BotInventories.UpdateAsync(botInventory);
                await _unitOfWork.Items.UpdateAsync(botInventory.Item);
                
                _logger.LogInformation($"Released trade lock for Item {botInventory.ItemId} (AssetId: {botInventory.Item.AssetId})");
            }

            var saved = await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"Successfully updated {saved} records in database.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing trade lock release job.");
            throw; 
        }
    }
}
