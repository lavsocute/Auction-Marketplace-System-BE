using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Interfaces;
using AuctionSys.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuctionSys.Infrastructure.Repositories;

public class BotRepository : AsyncRepository<Bot>, IBotRepository
{
    public BotRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Bot?> GetBySteamIdAsync(string steamId)
    {
        return await _dbContext.Bots.FirstOrDefaultAsync(b => b.SteamId == steamId);
    }
}
