namespace AuctionSys.Application.DTOs.Bot;

public class BotResponseDto
{
    public Guid Id { get; set; }
    public string SteamId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? TradeUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
