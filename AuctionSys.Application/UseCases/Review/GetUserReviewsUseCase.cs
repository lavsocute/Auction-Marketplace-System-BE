using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Review;
using AuctionSys.Application.Interfaces.UseCases.Review;
using AuctionSys.Domain.Interfaces;
using AutoMapper;

namespace AuctionSys.Application.UseCases.Review;

public class GetUserReviewsUseCase : IGetUserReviewsUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserReviewsUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<ReviewDto>>> ExecuteAsync(Guid userId)
    {
        // Lấy tất cả review mà người dùng này được nhận (với tư cách là Seller)
        var reviews = await _unitOfWork.Reviews.GetAsync(r => r.SellerId == userId);
        
        var dtos = _mapper.Map<List<ReviewDto>>(reviews);
        return ApiResponse<List<ReviewDto>>.Success(dtos);
    }
}
