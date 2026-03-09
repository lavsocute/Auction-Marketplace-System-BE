using AutoMapper;
using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Auction;
using AuctionSys.Application.Interfaces.UseCases.Auction;
using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Auction;

public class GetAuctionsUseCase : IGetAuctionsUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAuctionsUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResponse<AuctionDto>>> ExecuteAsync(int pageNumber = 1, int pageSize = 10, string? status = null)
    {
        AuctionStatus? parsedStatus = null;
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<AuctionStatus>(status, true, out var result))
        {
            parsedStatus = result;
        }

        var (auctions, totalAuctions) = await _unitOfWork.Auctions.GetPagedAuctionsAsync(pageNumber, pageSize, parsedStatus);

        var dtos = _mapper.Map<IEnumerable<AuctionDto>>(auctions).ToList();
        
        for (int i = 0; i < dtos.Count; i++)
        {
            if (auctions[i].Item != null && dtos[i].Item == null)
            {
                dtos[i].Item = _mapper.Map<AuctionSys.Application.DTOs.Item.ItemDto>(auctions[i].Item);
            }
        }

        return ApiResponse<PagedResponse<AuctionDto>>.Success(new PagedResponse<AuctionDto>
        {
            Items = dtos,
            TotalCount = totalAuctions,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }
}
