using AutoMapper;
using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Auction;
using AuctionSys.Application.Interfaces.UseCases.Auction;
using AuctionSys.Application.Interfaces.Services;
using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Auction;

public class CreateAuctionUseCase : ICreateAuctionUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IBackgroundJobService _backgroundJobService;

    public CreateAuctionUseCase(IUnitOfWork unitOfWork, IMapper mapper, IBackgroundJobService backgroundJobService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _backgroundJobService = backgroundJobService;
    }

    public async Task<ApiResponse<AuctionDto>> ExecuteAsync(Guid sellerId, CreateAuctionDto request)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(request.ItemId);
        
        if (item == null)
        {
            return ApiResponse<AuctionDto>.Fail("Sản phẩm không tồn tại.", 404);
        }

        if (item.SellerId != sellerId)
        {
            return ApiResponse<AuctionDto>.Fail("Bạn không có quyền mang sản phẩm này đi đấu giá.", 403);
        }

        if (item.ListType != ListType.Auction)
        {
            return ApiResponse<AuctionDto>.Fail("Sản phẩm này không mang cấu hình để đấu giá.", 400);
        }

        if (item.Status != ItemStatus.Available)
        {
            return ApiResponse<AuctionDto>.Fail("Sản phẩm không ở trạng thái khả dụng.", 400);
        }
        
        // Kiểm tra xem đã có Auction nào cho Item này chưa
        var existingAuctions = await _unitOfWork.Auctions.GetAsync(a => a.ItemId == request.ItemId);
        if (existingAuctions.Any(a => a.Status == AuctionStatus.Active))
        {
            return ApiResponse<AuctionDto>.Fail("Sản phẩm này đang trong một phiên đấu giá khác.", 400);
        }

        if (request.EndTime <= DateTime.UtcNow)
        {
            return ApiResponse<AuctionDto>.Fail("Thời gian kết thúc phải ở tương lai lớn hơn thời điểm hiện tại.", 400);
        }

        var auction = new AuctionSys.Domain.Entities.Auction
        {
            ItemId = request.ItemId,
            StartPrice = request.StartPrice,
            CurrentPrice = request.StartPrice,
            StartTime = DateTime.UtcNow,
            EndTime = request.EndTime,
            Status = AuctionStatus.Active
        };

        item.Status = ItemStatus.InAuction;

        await _unitOfWork.Auctions.AddAsync(auction);
        await _unitOfWork.Items.UpdateAsync(item);
        
        await _unitOfWork.CompleteAsync();

        // Lên lịch Hangfire background job để tự động đóng phiên khi hết hạn
        var delay = request.EndTime - DateTime.UtcNow;
        _backgroundJobService.Schedule<ICloseAuctionUseCase>(x => x.ExecuteAsync(auction.Id), delay);

        var dto = _mapper.Map<AuctionDto>(auction);
        dto.Item = _mapper.Map<AuctionSys.Application.DTOs.Item.ItemDto>(item);
        
        return ApiResponse<AuctionDto>.Success(dto, "Tạo phiên đấu giá thành công.", 201);
    }
}
