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

    public TopUpUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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
                wallet = new AuctionSys.Domain.Entities.Wallet { UserId = userId, Balance = 0, FrozenBalance = 0 };
                await _unitOfWork.Wallets.AddAsync(wallet);
            }

            wallet.Balance += request.Amount;

            var transaction = new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = request.Amount,
                Type = TransactionType.TopUp,
                Description = $"Nạp tiền vào tài khoản: +{request.Amount:N0} VNĐ"
            };

            await _unitOfWork.WalletTransactions.AddAsync(transaction);
            
            await _unitOfWork.CommitTransactionAsync();
            await _unitOfWork.CompleteAsync();

            var dto = _mapper.Map<WalletTransactionDto>(transaction);
            return ApiResponse<WalletTransactionDto>.Success(dto, "Nạp tiền thành công.");
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
