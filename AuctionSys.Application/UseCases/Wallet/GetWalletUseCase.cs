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
    private readonly IWalletSignatureService _signatureService;

    public GetWalletUseCase(IUnitOfWork unitOfWork, IMapper mapper, IWalletSignatureService signatureService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _signatureService = signatureService;
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
                UserId = userId
            };
            wallet.InitializeSignature(_signatureService);
            await _unitOfWork.Wallets.AddAsync(wallet);
            await _unitOfWork.CompleteAsync();
        }

        var dto = _mapper.Map<WalletDto>(wallet);
        return ApiResponse<WalletDto>.Success(dto);
    }
}
