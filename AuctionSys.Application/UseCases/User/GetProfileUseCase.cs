using AutoMapper;
using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.User;
using AuctionSys.Application.Interfaces.UseCases.User;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.User;

public class GetProfileUseCase : IGetProfileUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProfileUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UserProfileDto>> ExecuteAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
        {
            return ApiResponse<UserProfileDto>.Fail("Không tìm thấy người dùng.", 404);
        }

        var dto = _mapper.Map<UserProfileDto>(user);
        return ApiResponse<UserProfileDto>.Success(dto);
    }
}
