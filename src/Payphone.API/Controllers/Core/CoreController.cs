using Microsoft.AspNetCore.Mvc;
using Payphone.Application.Dtos.Core;
using Payphone.Application.Services.Core;

namespace Payphone.API.Controllers.Core;


/// <summary>
/// Controller for core entities
/// </summary>
/// <typeparam name="TService"></typeparam>
/// <typeparam name="TGet"></typeparam>
[ApiController]
public abstract class CoreController<TService, TGet> : ControllerBase
    where TService : IBaseService<TGet>
    where TGet : class
{
     private readonly TService _service;

    /// <summary>
    /// </summary>
    /// <param name="service"></param>
    protected CoreController(TService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all paginated
    /// </summary>
    /// <param name="search"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<IActionResult> Get([FromQuery] Paginate search, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetAllAsync(search, cancellationToken);
        return Ok(result);
    }



   /// <summary>
   /// get by id
   /// </summary>
   /// <param name="id"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        if (result.IsNotFound) return NotFound(result);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
    
    /// <summary>
    /// soft delete by Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _service.RemoveAsync(id, cancellationToken);
        if (result.IsNotFound) return NotFound(result);
        if (!result.IsSuccess) return BadRequest(result);
        return NoContent();
    }
    
}