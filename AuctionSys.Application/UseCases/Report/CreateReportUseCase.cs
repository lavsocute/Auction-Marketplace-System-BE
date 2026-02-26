using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Report;
using AuctionSys.Application.Interfaces.UseCases.Report;
using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;
using AutoMapper;

namespace AuctionSys.Application.UseCases.Report;

public class CreateReportUseCase : ICreateReportUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateReportUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ReportDto>> ExecuteAsync(Guid reporterId, CreateReportDto request)
    {
        // Kiểm tra đối tượng bị report có tồn tại không
        if (request.TargetType == ReportTargetType.User)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.TargetId);
            if (user == null)
                return ApiResponse<ReportDto>.Fail("Người dùng bị báo cáo không tồn tại.", 404);
            
            if (user.Id == reporterId)
                return ApiResponse<ReportDto>.Fail("Bạn không thể tự báo cáo chính mình.", 400);
        }
        else if (request.TargetType == ReportTargetType.Item)
        {
            var item = await _unitOfWork.Items.GetByIdAsync(request.TargetId);
            if (item == null)
                return ApiResponse<ReportDto>.Fail("Sản phẩm bị báo cáo không tồn tại.", 404);
            
            if (item.SellerId == reporterId)
                return ApiResponse<ReportDto>.Fail("Bạn không thể báo cáo sản phẩm của chính mình.", 400);
        }

        var report = new AuctionSys.Domain.Entities.Report
        {
            ReporterId = reporterId,
            TargetType = request.TargetType,
            TargetId = request.TargetId,
            Reason = request.Reason,
            Description = request.Description,
            Status = ReportStatus.Pending
        };

        await _unitOfWork.Reports.AddAsync(report);
        await _unitOfWork.CompleteAsync();

        var dto = _mapper.Map<ReportDto>(report);
        return ApiResponse<ReportDto>.Success(dto, "Báo cáo đã được gửi thành công và đang chờ xử lý.");
    }
}
