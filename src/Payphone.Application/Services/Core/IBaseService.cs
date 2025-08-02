namespace Payphone.Application.Services.Core;

public interface IBaseService<TGet>
    where TGet : class
{
    Task<Response<TGet>>  GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Response> RemoveAsync(int id, CancellationToken cancellationToken);
    Task<Response<PaginationResult<TGet>>> GetAllAsync(Paginate paginate,
        CancellationToken cancellationToken = default);
}