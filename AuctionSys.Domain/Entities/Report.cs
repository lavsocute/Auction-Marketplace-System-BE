using AuctionSys.Domain.Enums;

namespace AuctionSys.Domain.Entities;

public class Report
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReporterId { get; set; }
    public User Reporter { get; set; } = null!;

    public ReportTargetType TargetType { get; set; } // Item hoặc User
    public Guid TargetId { get; set; } // Id của Item hoặc User bị report

    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ReportStatus Status { get; set; } = ReportStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
}
