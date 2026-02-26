using AutoMapper;
using AuctionSys.Application.DTOs.Wallet;
using AuctionSys.Domain.Entities;

namespace AuctionSys.Application.Mappings;

public class WalletProfile : Profile
{
    public WalletProfile()
    {
        CreateMap<Wallet, WalletDto>();
        CreateMap<WalletTransaction, WalletTransactionDto>();
    }
}
