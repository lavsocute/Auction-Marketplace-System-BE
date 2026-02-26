using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Category;

namespace AuctionSys.Application.Interfaces.UseCases.Category;

public interface IGetAllCategoriesUseCase
{
    Task<ApiResponse<IEnumerable<CategoryDto>>> ExecuteAsync();
}
