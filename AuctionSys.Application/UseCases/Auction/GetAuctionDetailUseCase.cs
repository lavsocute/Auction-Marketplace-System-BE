using AutoMapper;
using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Auction;
using AuctionSys.Application.Interfaces.UseCases.Auction;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Auction;

public class GetAuctionDetailUseCase : IGetAuctionDetailUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAuctionDetailUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<AuctionDto>> ExecuteAsync(Guid auctionId)
    {
        var auction = await _unitOfWork.Auctions.GetByIdAsync(auctionId);
        if (auction == null)
        {
            return ApiResponse<AuctionDto>.Fail("Không tìm thấy phiên đấu giá.", 404);
        }

        var dto = _mapper.Map<AuctionDto>(auction);
        
        var item = await _unitOfWork.Items.GetByIdAsync(auction.ItemId);
        if (item != null)
        {
            dto.Item = _mapper.Map<AuctionSys.Application.DTOs.Item.ItemDto>(item);
        }

        if (auction.WinnerId.HasValue)
        {
            var winner = await _unitOfWork.Users.GetByIdAsync(auction.WinnerId.Value);
            if (winner != null)
            {
                dto.Winner = _mapper.Map<AuctionSys.Application.DTOs.User.UserProfileDto>(winner);
            }
        }

        return ApiResponse<AuctionDto>.Success(dto);
    }
}
