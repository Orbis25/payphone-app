using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Payphone.API.Controllers;

/// <summary>
/// User and Auth controller
/// </summary>
[ApiController]
[Route("api/users")]
[AllowAnonymous]
public class UsersController : ControllerBase
{
    public UsersController()
    {
    }

    /// <summary>
    /// endpoint to login user
    /// </summary>
    /// <returns></returns>
    [HttpPost("auth/login")]
    public async Task<IActionResult> Login()
    {
        return Ok();
    }
}