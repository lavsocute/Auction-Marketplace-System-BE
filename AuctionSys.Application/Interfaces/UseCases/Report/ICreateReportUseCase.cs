using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Report;

namespace AuctionSys.Application.Interfaces.UseCases.Report;

public interface ICreateReportUseCase
{
    Task<ApiResponse<ReportDto>> ExecuteAsync(Guid reporterId, CreateReportDto request);
}
