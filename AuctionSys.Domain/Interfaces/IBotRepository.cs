using AuctionSys.Domain.Entities;

namespace AuctionSys.Domain.Interfaces;

public interface IBotRepository : IAsyncRepository<Bot>
{
    Task<Bot?> GetBySteamIdAsync(string steamId);
}
