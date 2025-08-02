using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payphone.Application.Dtos.Core;
using Payphone.Application.Dtos.Users;
using Payphone.Application.Services.Users;

namespace Payphone.API.Controllers;

/// <summary>
/// User and Auth controller
/// </summary>
[ApiController]
[Route("api/users")]
[AllowAnonymous]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    /// <summary>
    /// endpoint to login user
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("auth/login")]
    [ProducesResponseType(typeof(Response<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto input, CancellationToken cancellationToken = default)
    {
        var response = await _service.LoginAsync(input, cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}