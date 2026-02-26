using System.ComponentModel.DataAnnotations;

namespace AuctionSys.Application.DTOs.Chat;

public class SendMessageDto
{
    [Required]
    public Guid ReceiverId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
}
