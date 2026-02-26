using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.User;

namespace AuctionSys.Application.Interfaces.UseCases.User;

public interface IGetProfileUseCase
{
    Task<ApiResponse<UserProfileDto>> ExecuteAsync(Guid userId);
}
