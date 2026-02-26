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
        var allAuctions = await _unitOfWork.Auctions.ListAllAsync();
        var query = allAuctions.AsEnumerable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<AuctionStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(a => a.Status == parsedStatus);
        }

        var totalAuctions = query.Count();

        var pagedAuctions = query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtos = _mapper.Map<IEnumerable<AuctionDto>>(pagedAuctions).ToList();
        
        // Manual Include Item cho Demo (Thực tế Join SQL query)
        foreach (var dto in dtos)
        {
            var item = await _unitOfWork.Items.GetByIdAsync(dto.ItemId);
            if (item != null)
                dto.Item = _mapper.Map<AuctionSys.Application.DTOs.Item.ItemDto>(item);
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
