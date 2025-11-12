using System.Linq;
using App.Domain.Models;
using App.Domain.Services;
using App.WebApi.Infrastructure;
using App.WebApi.Models.Requests;
using App.WebApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace App.WebApi.Controllers;

[ApiController]
[Route("api/v1/items")]
public sealed class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemsController(IItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _itemService.CreateAsync(new CreateItemModel(request.Name, request.Price), cancellationToken)
            .ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return this.ToActionResult(result.Error!);
        }

        var response = result.Value!.ToResponse();
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _itemService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccess)
        {
            return this.ToActionResult(result.Error!);
        }

        return Ok(result.Value!.ToResponse());
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken cancellationToken = default)
    {
        var result = await _itemService.ListAsync(skip, take, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccess)
        {
            return this.ToActionResult(result.Error!);
        }

        var items = result.Value!
            .Select(item => item.ToResponse())
            .ToList();

        return Ok(new ItemListResponse(items, skip, take));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _itemService.UpdateAsync(id, new UpdateItemModel(request.Name, request.Price), cancellationToken)
            .ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return this.ToActionResult(result.Error!);
        }

        return Ok(result.Value!.ToResponse());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _itemService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccess)
        {
            return this.ToActionResult(result.Error!);
        }

        return NoContent();
    }
}
