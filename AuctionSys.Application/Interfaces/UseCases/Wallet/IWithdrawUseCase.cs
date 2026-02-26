using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Wallet;

namespace AuctionSys.Application.Interfaces.UseCases.Wallet;

public interface IWithdrawUseCase
{
    Task<ApiResponse<WalletTransactionDto>> ExecuteAsync(Guid userId, WithdrawDto request);
}
