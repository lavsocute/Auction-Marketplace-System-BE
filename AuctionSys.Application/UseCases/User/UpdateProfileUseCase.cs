using AutoMapper;
using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.User;
using AuctionSys.Application.Interfaces.UseCases.User;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.User;

public class UpdateProfileUseCase : IUpdateProfileUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateProfileUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UserProfileDto>> ExecuteAsync(Guid userId, UpdateProfileDto request)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
        {
            return ApiResponse<UserProfileDto>.Fail("Không tìm thấy người dùng.", 404);
        }

        user.FullName = request.FullName;
        user.AvatarUrl = request.AvatarUrl;
        user.Bio = request.Bio;
        user.IsPublic = request.IsPublic;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.CompleteAsync();

        var dto = _mapper.Map<UserProfileDto>(user);
        return ApiResponse<UserProfileDto>.Success(dto, "Cập nhật hồ sơ thành công.");
    }
}
