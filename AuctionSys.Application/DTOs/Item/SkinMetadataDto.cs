using AuctionSys.Domain.Enums;

namespace AuctionSys.Application.DTOs.Item;

public class SkinMetadataDto
{
    public Guid Id { get; set; }
    public SkinExterior Exterior { get; set; }
    public decimal FloatValue { get; set; }
    public int PatternIndex { get; set; }
    public string? StickersJson { get; set; }
    public string? NameTag { get; set; }
    public bool StatTrak { get; set; }
}
