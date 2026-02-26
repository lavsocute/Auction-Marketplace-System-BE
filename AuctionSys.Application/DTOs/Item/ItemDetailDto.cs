using AuctionSys.Application.DTOs.User;

namespace AuctionSys.Application.DTOs.Item;

public class ItemDetailDto : ItemDto
{
    public UserProfileDto Seller { get; set; } = null!;
}
