using AuctionSys.Domain.Enums;

namespace AuctionSys.Application.DTOs.Bot;

public class DepositItemRequestDto
{
    public string AssetId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public ListType ListType { get; set; }
    public Guid? CategoryId { get; set; }
    
    // CS2 Metadata
    public SkinExterior Exterior { get; set; }
    public decimal? FloatValue { get; set; }
    public int? PatternIndex { get; set; }
    public bool IsStatTrak { get; set; }
    public string? NameTag { get; set; }
    public string? StickersJson { get; set; }
}
