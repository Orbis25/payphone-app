using Payphone.Application.Dtos.Wallets;
using Payphone.Application.Services.Core;

namespace Payphone.Application.Services.Wallets;

public interface IWalletService : IBaseService<WalletDto>
{
    Task<Response<int>> CreateAsync(CreateOrUpdateWallet input, CancellationToken cancellationToken);
    Task<Response> UpdateAsync(int id, CreateOrUpdateWallet input, CancellationToken cancellationToken = default);
}