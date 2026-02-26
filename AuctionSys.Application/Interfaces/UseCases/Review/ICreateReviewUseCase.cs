using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Review;

namespace AuctionSys.Application.Interfaces.UseCases.Review;

public interface ICreateReviewUseCase
{
    Task<ApiResponse<ReviewDto>> ExecuteAsync(Guid userId, CreateReviewDto request);
}
