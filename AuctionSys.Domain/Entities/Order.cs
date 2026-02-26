using AuctionSys.Domain.Enums;

namespace AuctionSys.Domain.Entities;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public Guid BuyerId { get; set; }
    public User Buyer { get; set; } = null!;

    public decimal TotalPrice { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Completed;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
