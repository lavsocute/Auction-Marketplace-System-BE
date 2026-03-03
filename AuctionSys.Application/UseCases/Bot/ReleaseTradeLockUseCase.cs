using AuctionSys.Domain.Enums;
using AuctionSys.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuctionSys.Application.UseCases.Bot;

public class ReleaseTradeLockUseCase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ReleaseTradeLockUseCase> _logger;

    public ReleaseTradeLockUseCase(AppDbContext context, ILogger<ReleaseTradeLockUseCase> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting Trade Lock Release job...");

        try
        {
            var now = DateTime.UtcNow;

            // Find items in bot inventory that are TradeLocked and time has expired
            // Also need to join with Item to update its status
            var expiredLocks = await _context.BotInventories
                .Include(bi => bi.Item)
                .Where(bi => bi.Item.Status == ItemStatus.TradeLocked && 
                             bi.TradeLockedUntil != null && 
                             bi.TradeLockedUntil <= now)
                .ToListAsync(cancellationToken);

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
                
                // Keep TradeLockedUntil value for history, but status indicates it's unlocked
                _logger.LogInformation($"Released trade lock for Item {botInventory.ItemId} (AssetId: {botInventory.Item.AssetId})");
            }

            var saved = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Successfully updated {saved} records in database.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing trade lock release job.");
            throw; // Hangfire will catch this and retry if configured
        }
    }
}
