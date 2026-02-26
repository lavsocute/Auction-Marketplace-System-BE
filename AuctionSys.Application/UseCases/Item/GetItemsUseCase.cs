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
        // Lấy tất cả Items từ DB (trong thực tế nên query IQueryable từ Repo để tránh load all)
        var allItems = await _unitOfWork.Items.ListAllAsync();
        
        var query = allItems.AsEnumerable();

        if (categoryId.HasValue)
        {
            query = query.Where(i => i.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim().ToLower();
            query = query.Where(i => i.Title.ToLower().Contains(searchLower) || 
                                     i.Description.ToLower().Contains(searchLower));
        }

        var totalItems = query.Count();
        
        var pagedItems = query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtos = _mapper.Map<IEnumerable<ItemDto>>(pagedItems);

        return ApiResponse<PagedResponse<ItemDto>>.Success(new PagedResponse<ItemDto>
        {
            Items = dtos,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }
}
