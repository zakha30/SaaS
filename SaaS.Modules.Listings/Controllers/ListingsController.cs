using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Listings.DTOs;
using SaaS.Modules.Listings.Entities;
using SaaS.Modules.Listings.Services;

namespace SaaS.Modules.Listings.Controllers;

[ApiController]
[Route("api/listings")]
[Authorize]
public sealed class ListingsController(IListingService service) : ControllerBase
{
    private Guid UserId =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User identity claim missing."));

    // ── Create ─────────────────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateListingDto dto, CancellationToken ct)
    {
        var result = await service.CreateAsync(dto, UserId, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById),
                new { id = result.Value!.Id }, result.Value)
            : BadRequest(new { error = result.Error });
    }

    // ── Read by id ─────────────────────────────────────────────────────────────

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await service.GetByIdAsync(id, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { error = result.Error });
    }

    // ── Paged (all listings for tenant, no filters) ────────────────────────────

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await service.GetPagedAsync(page, pageSize, ct);
        return Ok(result.Value);
    }

    // ── Search ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Full-featured search with keyword, location, type, price range,
    /// availability window, and sorting.
    ///
    /// Query string examples:
    ///   GET /api/listings/search?keyword=steel&amp;locationFrom=Istanbul&amp;sortBy=PriceAsc&amp;page=1&amp;pageSize=10
    ///   GET /api/listings/search?type=Truck&amp;maxPrice=5000&amp;sortBy=Newest
    ///   GET /api/listings/search?locationFrom=Ankara&amp;locationTo=Izmir&amp;minPrice=100&amp;maxPrice=9999
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] ListingSearchDto search, CancellationToken ct)
    {
        var result = await service.SearchAsync(search, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    // ── Update ─────────────────────────────────────────────────────────────────

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateListingDto dto, CancellationToken ct)
    {
        var result = await service.UpdateAsync(id, dto, UserId, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error!.Contains("permission")
                ? Forbid()
                : NotFound(new { error = result.Error });
    }

    // ── Status ─────────────────────────────────────────────────────────────────

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        Guid id, [FromBody] ChangeStatusDto dto, CancellationToken ct)
    {
        var result = await service.ChangeStatusAsync(id, dto.Status, UserId, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    // ── Delete ─────────────────────────────────────────────────────────────────

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await service.DeleteAsync(id, UserId, ct);
        return result.IsSuccess
            ? NoContent()
            : result.Error!.Contains("permission")
                ? Forbid()
                : NotFound(new { error = result.Error });
    }
}

public sealed record ChangeStatusDto(ListingStatus Status);