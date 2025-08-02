using System.Linq.Expressions;
using Payphone.Application.Dtos.Core;
using Payphone.Application.Repositories;
using Payphone.Infrastructure.EF.Repositories.Core;

namespace Payphone.Infrastructure.EF.Repositories;

public class WalletRepository: BaseRepository<ApplicationDbContext,Wallet> ,IWalletRepository
{
    public WalletRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override  async Task<PaginationResult<Wallet>> GetPaginatedListAsync(Paginate paginate, Expression<Func<Wallet, bool>>? expression = default,
        CancellationToken cancellationToken = default)
    {
        var results = GetAll(expression).OrderByDescending(x => x.CreatedAt).AsQueryable();

        if (!string.IsNullOrEmpty(paginate.Query))
        {
            paginate.Query = paginate.Query.ToLowerInvariant();

            results = results.Where(x => x.WalletCode.ToString().ToLower().Contains(paginate.Query) ||
                                         (!string.IsNullOrEmpty(x.OwnerDocumentId) && x.OwnerDocumentId.ToLower().Contains(paginate.Query)) ||
                                         (!string.IsNullOrEmpty(x.OwnerName) && x.OwnerName.ToLower().Contains(paginate.Query))
            );
            
        }

        if (paginate.NoPaginate)
        {
            return new()
            {
                Results = await results.AsNoTracking().ToListAsync(cancellationToken)
            };
        }

        var total = results.Count();
        var pages = (int)Math.Ceiling((decimal)total / paginate.Qyt);

        results = results.Skip((paginate.Page - 1) * paginate.Qyt).Take(paginate.Qyt);

        return new()
        {
            ActualPage = paginate.Page,
            Qyt = paginate.Qyt,
            PageTotal = pages,
            Total = total,
            Results = await results.AsNoTracking().ToListAsync(cancellationToken)
        };
    }
}