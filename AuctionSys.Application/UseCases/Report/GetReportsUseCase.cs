using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Report;
using AuctionSys.Application.Interfaces.UseCases.Report;
using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;
using AutoMapper;

namespace AuctionSys.Application.UseCases.Report;

public class GetReportsUseCase : IGetReportsUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetReportsUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResponse<ReportDto>>> ExecuteAsync(int pageNumber = 1, int pageSize = 10, ReportStatus? status = null)
    {
        var allReports = await _unitOfWork.Reports.ListAllAsync();
        var query = allReports.AsEnumerable();

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        var totalCount = query.Count();

        var pagedReports = query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtos = _mapper.Map<List<ReportDto>>(pagedReports);

        return ApiResponse<PagedResponse<ReportDto>>.Success(new PagedResponse<ReportDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        }, "Lấy danh sách báo cáo thành công.");
    }
}
