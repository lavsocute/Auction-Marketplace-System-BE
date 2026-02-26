using AuctionSys.Application.Common.Models;

namespace AuctionSys.Application.Interfaces.UseCases.Item;

public interface IPurchaseItemUseCase
{
    Task<ApiResponse<string>> ExecuteAsync(Guid buyerId, Guid itemId);
}
