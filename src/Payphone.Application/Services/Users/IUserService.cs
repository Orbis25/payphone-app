namespace Payphone.Application.Services.Users;

public interface IUserService
{
    Task<Response<LoginResponseDto>> LoginAsync(LoginDto input,
        CancellationToken cancellationToken = default);
}