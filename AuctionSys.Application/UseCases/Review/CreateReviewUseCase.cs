using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Review;
using AuctionSys.Application.Interfaces.UseCases.Review;
using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;
using AutoMapper;

namespace AuctionSys.Application.UseCases.Review;

public class CreateReviewUseCase : ICreateReviewUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateReviewUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ReviewDto>> ExecuteAsync(Guid userId, CreateReviewDto request)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
        if (order == null)
        {
            return ApiResponse<ReviewDto>.Fail("Đơn hàng không tồn tại.", 404);
        }

        if (order.BuyerId != userId)
        {
            return ApiResponse<ReviewDto>.Fail("Bạn không phải là người mua của đơn hàng này.", 403);
        }

        if (order.Status != OrderStatus.Completed)
        {
            return ApiResponse<ReviewDto>.Fail("Chỉ có thể đánh giá những đơn hàng đã hoàn tất.", 400);
        }

        var existingReviews = await _unitOfWork.Reviews.GetAsync(r => r.OrderId == request.OrderId && r.ReviewerId == userId);
        if (existingReviews.Any())
        {
            return ApiResponse<ReviewDto>.Fail("Bạn đã đánh giá đơn hàng này rồi.", 400);
        }

        var item = await _unitOfWork.Items.GetByIdAsync(order.ItemId);
        if (item == null)
        {
            return ApiResponse<ReviewDto>.Fail("Sản phẩm trong đơn hàng không còn tồn tại.", 404);
        }

        var review = new AuctionSys.Domain.Entities.Review
        {
            OrderId = request.OrderId,
            ReviewerId = userId,
            SellerId = item.SellerId,
            Rating = request.Rating,
            Comment = request.Comment
        };

        await _unitOfWork.Reviews.AddAsync(review);
        
        await _unitOfWork.Notifications.AddAsync(new AuctionSys.Domain.Entities.Notification
        {
            UserId = item.SellerId,
            Type = NotificationType.ReviewReceived,
            Title = "Bạn có một đánh giá mới!",
            Message = $"Người dùng vừa để lại đánh giá {request.Rating} sao cho đơn hàng '{item.Title}'.",
            ReferenceId = review.Id.ToString()
        });

        await _unitOfWork.CompleteAsync();

        var dto = _mapper.Map<ReviewDto>(review);
        return ApiResponse<ReviewDto>.Success(dto, "Đánh giá thành công!");
    }
}
