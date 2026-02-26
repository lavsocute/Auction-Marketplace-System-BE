using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuctionSys.Application.DTOs.Wallet;
using AuctionSys.Application.Interfaces.UseCases.Wallet;

namespace AuctionSys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletsController : ControllerBase
{
    private readonly IGetWalletUseCase _getWalletUseCase;
    private readonly ITopUpUseCase _topUpUseCase;
    private readonly IWithdrawUseCase _withdrawUseCase;
    private readonly IGetTransactionHistoryUseCase _getTransactionHistoryUseCase;

    public WalletsController(
        IGetWalletUseCase getWalletUseCase,
        ITopUpUseCase topUpUseCase,
        IWithdrawUseCase withdrawUseCase,
        IGetTransactionHistoryUseCase getTransactionHistoryUseCase)
    {
        _getWalletUseCase = getWalletUseCase;
        _topUpUseCase = topUpUseCase;
        _withdrawUseCase = withdrawUseCase;
        _getTransactionHistoryUseCase = getTransactionHistoryUseCase;
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new UnauthorizedAccessException("Không tìm thấy thông tin UserId trong token.");
        return Guid.Parse(userIdStr);
    }

    [HttpGet]
    public async Task<IActionResult> GetWallet()
    {
        var response = await _getWalletUseCase.ExecuteAsync(GetUserId());
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("topup")]
    public async Task<IActionResult> TopUp([FromBody] TopUpDto request)
    {
        var response = await _topUpUseCase.ExecuteAsync(GetUserId(), request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawDto request)
    {
        var response = await _withdrawUseCase.ExecuteAsync(GetUserId(), request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactionHistory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _getTransactionHistoryUseCase.ExecuteAsync(GetUserId(), pageNumber, pageSize);
        return StatusCode(response.StatusCode, response);
    }
}
