namespace AuctionSys.Application.DTOs.Review;

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ReviewerId { get; set; }
    public Guid SellerId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } // Giả sử có CreatedAt, nếu entity không có thì auto mapper tự skip hoặc thêm vào entity
}
