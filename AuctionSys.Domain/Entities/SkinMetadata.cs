using AuctionSys.Domain.Enums;

namespace AuctionSys.Domain.Entities;

public class SkinMetadata
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;
    
    public SkinExterior Exterior { get; set; } = SkinExterior.NotApplicable;
    public decimal? FloatValue { get; set; }
    public int? PatternIndex { get; set; }
    public bool IsStatTrak { get; set; } = false;
    public string? NameTag { get; set; }
    
    // Stickers could be stored as JSON string or a simple comma separated string for now
    public string? StickersJson { get; set; }
}
