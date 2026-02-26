using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Wallet;

namespace AuctionSys.Application.Interfaces.UseCases.Wallet;

public interface IGetWalletUseCase
{
    Task<ApiResponse<WalletDto>> ExecuteAsync(Guid userId);
}
