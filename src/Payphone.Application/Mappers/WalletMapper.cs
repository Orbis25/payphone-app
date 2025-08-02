
using Payphone.Application.Dtos.Wallets;

namespace Payphone.Application.Mappers;

public class WalletMapper : Profile
{
    public WalletMapper()
    {
        CreateMap<Wallet, WalletDto>().ReverseMap();
        CreateMap<Wallet, CreateOrUpdateWallet>().ReverseMap();
        CreateMap<WalletTransaction, WalletTransactionDto>().ReverseMap();
    }
}