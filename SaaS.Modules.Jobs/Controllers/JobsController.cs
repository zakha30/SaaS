using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Jobs.DTOs;
using SaaS.Modules.Jobs.Services;
using SaaS.Modules.Tenants.Http;
using SaaS.Modules.Tenants.Repositories;
using SaaS.Shared;

namespace SaaS.Modules.Jobs.Controllers;

[ApiController]
[Route("api/jobs")]
public sealed class JobsController(
    IJobService service,
    ITenantContext tenantContext,
    ITenantRepository tenantRepository) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get(
        [FromQuery] string? tenantSlug,
        [FromQuery] JobFilterDto filter,
        CancellationToken ct)
    {
        var err = await PublicTenantResolver.TryResolveForPublicDataAsync(
            this, tenantContext, tenantRepository, tenantSlug, ct);
        if (err is not null) return err;

        var paged = await service.GetFilteredAsync(filter, ct);
        return Ok(paged);
    }

    [HttpPost]
    [Authorize(Policy = "TransporterOrAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateJobDto dto, CancellationToken ct)
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

        var job = await service.GetByIdAsync(id, ct);
        return job is null ? NotFound() : Ok(job);
    }
}
