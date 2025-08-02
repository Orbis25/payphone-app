using System.Linq.Expressions;
using Payphone.Domain.Core.Models;

namespace Payphone.Application.Repositories.Core;

public interface IBaseRepository<TModel>
    where TModel : BaseModel
{
    Task<PaginationResult<TModel>> GetPaginatedListAsync(Paginate paginate,
        Expression<Func<TModel, bool>>? expression = default, 
        CancellationToken cancellationToken = default);
    IQueryable<TModel> GetAll(Expression<Func<TModel, bool>>? expression = default);
    Task<TModel> CreateAsync(TModel model, CancellationToken cancellationToken = default);
    TModel Attach(TModel model);
    Task<TModel?> UpdateAsync(TModel model, CancellationToken cancellationToken = default);
    Task<bool> SoftRemoveAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistAsync(Expression<Func<TModel, bool>>? expression = default, CancellationToken cancellationToken = default);
    Task<TModel?> GetOneAsync(Expression<Func<TModel, bool>> expression, CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
}