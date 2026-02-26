using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuctionSys.Application.DTOs.Item;
using AuctionSys.Application.Interfaces.UseCases.Item;

namespace AuctionSys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IGetItemsUseCase _getItemsUseCase;
    private readonly ICreateItemUseCase _createItemUseCase;
    private readonly IGetItemDetailUseCase _getItemDetailUseCase;
    private readonly IPurchaseItemUseCase _purchaseItemUseCase;

    public ItemsController(
        IGetItemsUseCase getItemsUseCase,
        ICreateItemUseCase createItemUseCase,
        IGetItemDetailUseCase getItemDetailUseCase,
        IPurchaseItemUseCase purchaseItemUseCase)
    {
        _getItemsUseCase = getItemsUseCase;
        _createItemUseCase = createItemUseCase;
        _getItemDetailUseCase = getItemDetailUseCase;
        _purchaseItemUseCase = purchaseItemUseCase;
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new UnauthorizedAccessException("Không tìm thấy thông tin UserId trong token.");
        return Guid.Parse(userIdStr);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetItems(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] Guid? categoryId = null, 
        [FromQuery] string? search = null)
    {
        var response = await _getItemsUseCase.ExecuteAsync(pageNumber, pageSize, categoryId, search);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetItemDetail(Guid id)
    {
        var response = await _getItemDetailUseCase.ExecuteAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateItem([FromBody] CreateItemDto request)
    {
        var response = await _createItemUseCase.ExecuteAsync(GetUserId(), request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("{id}/buy")]
    [Authorize]
    public async Task<IActionResult> BuyItem(Guid id)
    {
        var response = await _purchaseItemUseCase.ExecuteAsync(GetUserId(), id);
        return StatusCode(response.StatusCode, response);
    }
}
