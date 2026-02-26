using AutoMapper;
using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Wallet;
using AuctionSys.Application.Interfaces.UseCases.Wallet;
using AuctionSys.Domain.Entities;
using AuctionSys.Domain.Enums;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Wallet;

public class WithdrawUseCase : IWithdrawUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WithdrawUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<WalletTransactionDto>> ExecuteAsync(Guid userId, WithdrawDto request)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var wallets = await _unitOfWork.Wallets.GetAsync(w => w.UserId == userId);
            var wallet = wallets.FirstOrDefault();

            // Kiểm tra số dư khả dụng (Balance hiện tại vốn đã không bao gồm tiền bị đóng băng)
            if (wallet == null || wallet.Balance < request.Amount)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<WalletTransactionDto>.Fail("Số dư trong ví không đủ để thực hiện rút tiền.");
            }

            wallet.Balance -= request.Amount;

            var transaction = new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = -request.Amount,
                Type = TransactionType.Withdraw,
                Description = $"Rút tiền về ngân hàng {request.BankName} - STK: {request.BankAccountNumber} (-{request.Amount:N0} VNĐ)"
            };

            await _unitOfWork.WalletTransactions.AddAsync(transaction);
            
            await _unitOfWork.CommitTransactionAsync();
            await _unitOfWork.CompleteAsync();

            var dto = _mapper.Map<WalletTransactionDto>(transaction);
            return ApiResponse<WalletTransactionDto>.Success(dto, "Yêu cầu rút tiền được thực hiện thành công.");
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
