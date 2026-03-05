using AuctionSys.Application.DTOs.Bot;

namespace AuctionSys.Application.DTOs.Item;

public class MarketplaceItemDto : ItemDto
{
    public SkinMetadataDto? SkinMetadata { get; set; }
    public DateTime? TradeLockedUntil { get; set; }
}
