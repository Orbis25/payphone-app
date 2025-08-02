using Payphone.Application.Services.Core;

namespace Payphone.Application.Services.Users;

public interface IUserService : ISeed
{
    Task<Response<LoginResponseDto>> LoginAsync(LoginDto input,
        CancellationToken cancellationToken = default);
}