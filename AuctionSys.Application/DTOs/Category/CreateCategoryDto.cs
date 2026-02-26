using System.ComponentModel.DataAnnotations;

namespace AuctionSys.Application.DTOs.Category;

public class CreateCategoryDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(2048)]
    public string IconUrl { get; set; } = string.Empty;
}
