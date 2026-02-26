using System.ComponentModel.DataAnnotations;
using AuctionSys.Domain.Enums;

namespace AuctionSys.Application.DTOs.Item;

public class CreateItemDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    [Required]
    [Range(1000, 1000000000)]
    public decimal Price { get; set; }

    [Required]
    public ListType ListType { get; set; }

    [Required]
    public Guid CategoryId { get; set; }
}
