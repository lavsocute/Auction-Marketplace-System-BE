using AutoMapper;
using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Wallet;
using AuctionSys.Application.Interfaces.UseCases.Wallet;
using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Wallet;

public class TopUpUseCase : ITopUpUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IWalletSignatureService _signatureService;

    public TopUpUseCase(IUnitOfWork unitOfWork, IMapper mapper, IWalletSignatureService signatureService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _signatureService = signatureService;
    }

    public async Task<ApiResponse<WalletTransactionDto>> ExecuteAsync(Guid userId, TopUpDto request)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var wallets = await _unitOfWork.Wallets.GetAsync(w => w.UserId == userId);
            var wallet = wallets.FirstOrDefault();

            if (wallet == null)
            {
                wallet = new AuctionSys.Domain.Entities.Wallet { UserId = userId };
                wallet.InitializeSignature(_signatureService);
                await _unitOfWork.Wallets.AddAsync(wallet);
            }

            // Centralized Append-Only Ledger process
            wallet.ProcessTransaction(request.Amount, TransactionType.TopUp, $"Nạp tiền vào tài khoản: +{request.Amount:N0} VNĐ", _signatureService);

            // Access the generated transaction record from the wallet collection for DTO mapping
            var transaction = wallet.Transactions.Last();
            
            await _unitOfWork.CommitTransactionAsync();
            await _unitOfWork.CompleteAsync();

            var dto = _mapper.Map<WalletTransactionDto>(transaction);
            return ApiResponse<WalletTransactionDto>.Success(dto, "Nạp tiền thành công.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw new Exception($"Top up error: {ex.Message}");
        }
    }
}
