using AuctionSys.Domain.Enums;
namespace AuctionSys.Application.DTOs.Item;

public class MarketplaceItemFilterDto
{
    public decimal? MinFloat { get; set; }
    public decimal? MaxFloat { get; set; }
    public int? PatternIndex { get; set; }
    public SkinExterior? Exterior { get; set; }
    public bool? IsStatTrak { get; set; }
    public bool? HasStickers { get; set; }
    public string? SearchTerm { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public MarketplaceSortBy? SortBy { get; set; }

    // Pagination
    public string? Cursor { get; set; }
    public int PageSize { get; set; } = 20;
}

public enum MarketplaceSortBy
{
    Newest,
    PriceAsc,
    PriceDesc,
    FloatAsc
}
