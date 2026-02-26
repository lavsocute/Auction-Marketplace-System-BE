using AuctionSys.Domain.Enums;

namespace AuctionSys.Application.DTOs.Item;

public class ItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public ListType ListType { get; set; }
    public ItemStatus Status { get; set; }
    public Guid SellerId { get; set; }
    public Guid? CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
}
