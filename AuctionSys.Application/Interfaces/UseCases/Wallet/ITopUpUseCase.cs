using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Wallet;

namespace AuctionSys.Application.Interfaces.UseCases.Wallet;

public interface ITopUpUseCase
{
    Task<ApiResponse<WalletTransactionDto>> ExecuteAsync(Guid userId, TopUpDto request);
}
