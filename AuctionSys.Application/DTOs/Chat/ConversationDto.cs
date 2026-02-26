namespace AuctionSys.Application.DTOs.Chat;

public class ConversationDto
{
    public Guid PartnerId { get; set; }
    public string PartnerName { get; set; } = string.Empty;
    public string PartnerAvatarUrl { get; set; } = string.Empty;
    public ChatMessageDto? LastMessage { get; set; }
    public int UnreadCount { get; set; }
}
