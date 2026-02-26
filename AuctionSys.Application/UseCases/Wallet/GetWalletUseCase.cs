using AutoMapper;
using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Wallet;
using AuctionSys.Application.Interfaces.UseCases.Wallet;
using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Wallet;

public class GetWalletUseCase : IGetWalletUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetWalletUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<WalletDto>> ExecuteAsync(Guid userId)
    {
        var wallets = await _unitOfWork.Wallets.GetAsync(w => w.UserId == userId);
        var wallet = wallets.FirstOrDefault();

        // Lazy creation if not exists
        if (wallet == null)
        {
            wallet = new AuctionSys.Domain.Entities.Wallet
            {
                UserId = userId,
                Balance = 0,
                FrozenBalance = 0
            };
            await _unitOfWork.Wallets.AddAsync(wallet);
            await _unitOfWork.CompleteAsync();
        }

        var dto = _mapper.Map<WalletDto>(wallet);
        return ApiResponse<WalletDto>.Success(dto);
    }
}
