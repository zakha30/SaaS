using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Loads.DTOs;
using SaaS.Modules.Loads.Services;
using SaaS.Modules.Tenants.Http;
using SaaS.Modules.Tenants.Repositories;
using SaaS.Shared;

namespace SaaS.Modules.Loads.Controllers;

[ApiController]
[Route("api/available-loads")]
public sealed class AvailableLoadsController(
    IAvailableLoadService service,
    ITenantContext tenantContext,
    ITenantRepository tenantRepository) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get(
        [FromQuery] string? tenantSlug,
        [FromQuery] LoadFilterDto filter,
        CancellationToken ct)
    {
        var err = await PublicTenantResolver.TryResolveForPublicDataAsync(
            this, tenantContext, tenantRepository, tenantSlug, ct);
        if (err is not null) return err;

        var paged = await service.GetFilteredAsync(filter, ct);
        return Ok(paged);
    }

    [HttpPost]
    [Authorize(Policy = "ShipperOrAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateLoadDto dto, CancellationToken ct)
    {
        var userId = Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User identity claim missing."));
        var created = await service.CreateAsync(dto, userId, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromQuery] string? tenantSlug,
        CancellationToken ct)
    {
        var err = await PublicTenantResolver.TryResolveForPublicDataAsync(
            this, tenantContext, tenantRepository, tenantSlug, ct);
        if (err is not null) return err;

        _ = id;
        return NotFound();
    }
}
