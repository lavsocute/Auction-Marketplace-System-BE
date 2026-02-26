using AuctionSys.Application.Common.Models;
using AuctionSys.Application.Interfaces.UseCases.Report;
using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Report;

public class ResolveReportUseCase : IResolveReportUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public ResolveReportUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<string>> ExecuteAsync(Guid reportId, string resolutionNotes)
    {
        var report = await _unitOfWork.Reports.GetByIdAsync(reportId);
        if (report == null)
        {
            return ApiResponse<string>.Fail("Báo cáo không tồn tại.", 404);
        }

        if (report.Status == ReportStatus.Resolved)
        {
            return ApiResponse<string>.Fail("Báo cáo này đã được xử lý.", 400);
        }

        report.Status = ReportStatus.Resolved;
        // Optionally update a ResolutionNotes or ResolvedAt field here if it exists in the entity 
        // For now, we mainly update the Status.

        await _unitOfWork.Reports.UpdateAsync(report);
        await _unitOfWork.CompleteAsync();

        return ApiResponse<string>.Success("Báo cáo đã được đánh dấu xử lý thành công.");
    }
}
