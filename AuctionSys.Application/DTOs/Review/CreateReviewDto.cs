using System.ComponentModel.DataAnnotations;

namespace AuctionSys.Application.DTOs.Review;

public class CreateReviewDto
{
    [Required]
    public Guid OrderId { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Rating phải từ 1 đến 5 sao")]
    public int Rating { get; set; }

    [MaxLength(500)]
    public string Comment { get; set; } = string.Empty;
}
