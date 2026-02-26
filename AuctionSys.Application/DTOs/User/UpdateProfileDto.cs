using System.ComponentModel.DataAnnotations;

namespace AuctionSys.Application.DTOs.User;

public class UpdateProfileDto
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    [MaxLength(500)]
    public string? Bio { get; set; }

    public bool IsPublic { get; set; }
}
