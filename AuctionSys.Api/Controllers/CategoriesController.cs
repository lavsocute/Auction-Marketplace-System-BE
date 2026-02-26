using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuctionSys.Application.DTOs.Category;
using AuctionSys.Application.Interfaces.UseCases.Category;

namespace AuctionSys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IGetAllCategoriesUseCase _getAllCategoriesUseCase;
    private readonly ICreateCategoryUseCase _createCategoryUseCase;

    public CategoriesController(
        IGetAllCategoriesUseCase getAllCategoriesUseCase,
        ICreateCategoryUseCase createCategoryUseCase)
    {
        _getAllCategoriesUseCase = getAllCategoriesUseCase;
        _createCategoryUseCase = createCategoryUseCase;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllCategories()
    {
        var response = await _getAllCategoriesUseCase.ExecuteAsync();
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto request)
    {
        var response = await _createCategoryUseCase.ExecuteAsync(request);
        return StatusCode(response.StatusCode, response);
    }
}
