using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Review;

namespace AuctionSys.Application.Interfaces.UseCases.Review;

public interface IGetUserReviewsUseCase
{
    Task<ApiResponse<List<ReviewDto>>> ExecuteAsync(Guid userId);
}
