using System.ComponentModel.DataAnnotations;
using AuctionSys.Domain.Enums;

namespace AuctionSys.Application.DTOs.Report;

public class CreateReportDto
{
    [Required]
    public ReportTargetType TargetType { get; set; }

    [Required]
    public Guid TargetId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
}
