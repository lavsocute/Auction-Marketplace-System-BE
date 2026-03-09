using AutoMapper;
using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Item;
using AuctionSys.Application.Interfaces.UseCases.Item;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Item;

public class GetItemsUseCase : IGetItemsUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetItemsUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResponse<ItemDto>>> ExecuteAsync(
        int pageNumber = 1, 
        int pageSize = 10, 
        Guid? categoryId = null, 
        string? search = null)
    {
        var (items, totalCount) = await _unitOfWork.Items.GetPagedItemsAsync(pageNumber, pageSize, categoryId, search);

        var dtos = _mapper.Map<IEnumerable<ItemDto>>(items);

        return ApiResponse<PagedResponse<ItemDto>>.Success(new PagedResponse<ItemDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }
}
