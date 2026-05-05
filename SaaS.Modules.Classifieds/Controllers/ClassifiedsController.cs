using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Classifieds.DTOs;
using SaaS.Modules.Classifieds.Services;
using SaaS.Modules.Tenants.Http;
using SaaS.Modules.Tenants.Repositories;
using SaaS.Shared;

namespace SaaS.Modules.Classifieds.Controllers;

[ApiController]
[Route("api/classifieds")]
public sealed class ClassifiedsController(
    IClassifiedService service,
    ITenantContext tenantContext,
    ITenantRepository tenantRepository) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get(
        [FromQuery] string? tenantSlug,
        [FromQuery] ClassifiedFilterDto filter,
        CancellationToken ct)
    {
        var err = await PublicTenantResolver.TryResolveForPublicDataAsync(
            this, tenantContext, tenantRepository, tenantSlug, ct);
        if (err is not null) return err;

        var paged = await service.GetFilteredAsync(filter, ct);
        return Ok(paged);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateClassifiedDto dto, CancellationToken ct)
    {
        var created = await service.CreateAsync(dto, ct);
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

        var ad = await service.GetByIdAsync(id, ct);
        return ad is null ? NotFound() : Ok(ad);
    }
}
