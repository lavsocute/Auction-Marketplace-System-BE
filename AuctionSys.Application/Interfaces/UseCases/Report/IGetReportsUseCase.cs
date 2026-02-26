using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Report;
using AuctionSys.Domain.Enums;

namespace AuctionSys.Application.Interfaces.UseCases.Report;

public interface IGetReportsUseCase
{
    Task<ApiResponse<PagedResponse<ReportDto>>> ExecuteAsync(int pageNumber = 1, int pageSize = 10, ReportStatus? status = null);
}
