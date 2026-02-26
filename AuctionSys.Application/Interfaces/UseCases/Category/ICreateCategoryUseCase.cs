using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Category;

namespace AuctionSys.Application.Interfaces.UseCases.Category;

public interface ICreateCategoryUseCase
{
    Task<ApiResponse<CategoryDto>> ExecuteAsync(CreateCategoryDto request);
}
