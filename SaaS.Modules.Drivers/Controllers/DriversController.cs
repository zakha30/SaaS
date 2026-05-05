using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Drivers.DTOs;
using SaaS.Modules.Drivers.Services;
using SaaS.Modules.Tenants.Http;
using SaaS.Modules.Tenants.Repositories;
using SaaS.Shared;
using System;

namespace SaaS.Modules.Drivers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class DriversController(
        IDriverService service,
        ITenantContext tenantContext,
        ITenantRepository tenantRepository) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PagedResult<DriverResponseDto>), 200)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? tenantSlug,
            [FromQuery] DriverFilterDto filter,
            CancellationToken ct)
        {
            var err = await PublicTenantResolver.TryResolveForPublicDataAsync(
                this, tenantContext, tenantRepository, tenantSlug, ct);
            if (err is not null) return err;

            var res = await service.GetFilteredAsync(filter, ct);
            return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(DriverResponseDto), 200)]
        public async Task<IActionResult> GetById(
            Guid id,
            [FromQuery] string? tenantSlug,
            CancellationToken ct)
        {
            var err = await PublicTenantResolver.TryResolveForPublicDataAsync(
                this, tenantContext, tenantRepository, tenantSlug, ct);
            if (err is not null) return err;

            var res = await service.GetByIdAsync(id, ct);
            return res.IsSuccess ? Ok(res.Value) : NotFound(res.Error);
        }

        [HttpPost]
        [Authorize(Policy = "FleetOwnerOrAdmin")]
        [ProducesResponseType(typeof(DriverResponseDto), 201)]
        public async Task<IActionResult> Create(
            [FromBody] CreateDriverDto dto,
            CancellationToken ct)
        {
            var userId = Guid.Parse(User.Identity?.Name ?? Guid.Empty.ToString());
            var res = await service.CreateAsync(dto, userId, ct);
            return res.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = res.Value!.Id }, res.Value)
                : BadRequest(new { error = res.Error });
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "FleetOwnerOrAdmin")]
        [ProducesResponseType(typeof(DriverResponseDto), 200)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateDriverDto dto,
            CancellationToken ct)
        {
            var userId = Guid.Parse(User.Identity?.Name ?? Guid.Empty.ToString());
            var res = await service.UpdateAsync(id, dto, userId, ct);
            return res.IsSuccess ? Ok(res.Value) : BadRequest(new { error = res.Error });
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "FleetOwnerOrAdmin")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var userId = Guid.Parse(User.Identity?.Name ?? Guid.Empty.ToString());
            var res = await service.DeleteAsync(id, userId, ct);
            return res.IsSuccess ? Ok(new { message = res.Value }) : BadRequest(new { error = res.Error });
        }
    }
}
