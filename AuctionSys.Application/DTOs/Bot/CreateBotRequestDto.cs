namespace AuctionSys.Application.DTOs.Bot;

public class CreateBotRequestDto
{
    public string SteamId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? TradeUrl { get; set; }
}
