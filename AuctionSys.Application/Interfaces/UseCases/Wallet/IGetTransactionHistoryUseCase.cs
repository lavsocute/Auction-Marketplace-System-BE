using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Wallet;

namespace AuctionSys.Application.Interfaces.UseCases.Wallet;

public interface IGetTransactionHistoryUseCase
{
    Task<ApiResponse<PagedResponse<WalletTransactionDto>>> ExecuteAsync(Guid userId, int pageNumber = 1, int pageSize = 10);
}
