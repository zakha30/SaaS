using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Directory.DTOs;
using SaaS.Modules.Directory.Services;
using SaaS.Modules.Tenants.Http;
using SaaS.Modules.Tenants.Repositories;
using SaaS.Shared;

namespace SaaS.Modules.Directory.Controllers;

[ApiController]
[Route("api/directory")]
public sealed class DirectoryController(
    IDirectoryService service,
    ITenantContext tenantContext,
    ITenantRepository tenantRepository) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get(
        [FromQuery] string? tenantSlug,
        [FromQuery] DirectoryFilterDto filter,
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
    public async Task<IActionResult> Create([FromBody] CreateDirectoryEntryDto dto, CancellationToken ct)
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

        var entry = await service.GetByIdAsync(id, ct);
        return entry is null ? NotFound() : Ok(entry);
    }
}
