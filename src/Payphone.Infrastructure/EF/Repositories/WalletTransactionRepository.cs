using Payphone.Application.Repositories;
using Payphone.Infrastructure.EF.Repositories.Core;

namespace Payphone.Infrastructure.EF.Repositories;

public class WalletTransactionRepository : BaseRepository<ApplicationDbContext, WalletTransaction> , IWalletTransactionRepository
{
    public WalletTransactionRepository(ApplicationDbContext context) : base(context)
    {
    }
}