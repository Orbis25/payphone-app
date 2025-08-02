using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payphone.API.Controllers.Core;
using Payphone.Application.Dtos.Core;
using Payphone.Application.Dtos.Wallets;
using Payphone.Application.Services.Wallets;

namespace Payphone.API.Controllers;

/// <summary>
/// controller for wallets operations
/// </summary>
[Route("api/wallets")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class WalletController : CoreController<IWalletService, WalletDto>
{
    private readonly IWalletService _service;

    public WalletController(IWalletService service) : base(service)
    {
        _service = service;
    }

    /// <summary>
    /// Create a new 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(Response<int>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateOrUpdateWallet input,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.CreateAsync(input, cancellationToken);

        if (!result.IsSuccess) return BadRequest(result);

        return CreatedAtAction(nameof(Get), new { Id = result.Data }, result);
    }

    /// <summary>
    /// update 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Response), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] CreateOrUpdateWallet input,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.UpdateAsync(id, input, cancellationToken);

        if (!result.IsSuccess) return BadRequest(result);

        return NoContent();
    }
}