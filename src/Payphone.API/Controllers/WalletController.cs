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

    /// <summary>
    /// create transaction for a wallet
    /// </summary>
    /// <remarks>
    /// Rules for processing transactions:
    /// 
    /// Debit Transaction:
    /// - Type must be TransactionType.Debit.
    /// - Requires ToWalletId (destination wallet).
    /// - Source wallet (fromWallet) must have sufficient balance.
    /// - Deducts amount from source wallet and adds to destination wallet.
    /// 
    /// Credit Transaction:
    /// - Type must be TransactionType.Credit.
    /// - ToWalletId is not required.
    /// - Increases balance of the source wallet.
    /// 
    /// General Considerations:
    /// - All referenced wallets must exist.
    /// - Transfers to the same wallet are not allowed for debits.
    /// - Transaction is recorded and balances are updated.
    /// - Errors are handled and logged appropriately.
    /// </remarks>
    /// <param name="walletId">walletId</param>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{fromWalletId:int}/transactions")]
    [ProducesResponseType(typeof(Response<CreateWalletTransactionResultDto>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTransactionAsync([FromRoute] int fromWalletId,
        [FromBody] CreateWalletTransactionDto input,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.CreateTransactionAsync(fromWalletId, input, cancellationToken);

        if (result.IsNotFound) return NotFound(result);

        if (!result.IsSuccess) return BadRequest(result);

        return Ok(result);
    }


    /// <summary>
    ///  get transactions for a wallet without authentication
    /// </summary>
    /// <param name="fromWalletId"></param>
    /// <param name="paginate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("{walletId:int}/transactions")]
    [ProducesResponseType(typeof(Response<PaginationResult<WalletTransactionDto>>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTransactionsAsync([FromRoute] int walletId,
        [FromQuery] Paginate paginate,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetTransactionsAsync(walletId, paginate, cancellationToken);

        if (result.IsNotFound) return NotFound(result);

        if (!result.IsSuccess) return BadRequest(result);

        return Ok(result);
    }
}