using AutoMapper;
using AuctionSys.Application.Common.Models;
using AuctionSys.Application.DTOs.Wallet;
using AuctionSys.Application.Interfaces.UseCases.Wallet;
using AuctionSys.Domain.Interfaces;

namespace AuctionSys.Application.UseCases.Wallet;

public class GetTransactionHistoryUseCase : IGetTransactionHistoryUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTransactionHistoryUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResponse<WalletTransactionDto>>> ExecuteAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
    {
        var wallets = await _unitOfWork.Wallets.GetAsync(w => w.UserId == userId);
        var wallet = wallets.FirstOrDefault();

        if (wallet == null)
        {
            return ApiResponse<PagedResponse<WalletTransactionDto>>.Success(new PagedResponse<WalletTransactionDto>
            {
                TotalCount = 0,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        var allTransactions = await _unitOfWork.WalletTransactions.GetAsync(t => t.WalletId == wallet.Id);
        
        // Sắp xếp giảm dần theo thời gian tạo, phân trang
        var pagedTransactions = allTransactions
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtos = _mapper.Map<IEnumerable<WalletTransactionDto>>(pagedTransactions);

        var pagedResponse = new PagedResponse<WalletTransactionDto>
        {
            Items = dtos,
            TotalCount = allTransactions.Count,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return ApiResponse<PagedResponse<WalletTransactionDto>>.Success(pagedResponse);
    }
}
