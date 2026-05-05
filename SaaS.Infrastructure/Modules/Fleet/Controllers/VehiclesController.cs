using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaaS.Infrastructure.Modules.Fleet.DTOs;
using SaaS.Infrastructure.Modules.Fleet.Services;
using SaaS.Modules.Tenants.Http;
using SaaS.Modules.Tenants.Repositories;
using SaaS.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SaaS.Infrastructure.Modules.Fleet.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class VehiclesController : ControllerBase
{
    private readonly IVehicleService _service;
    private readonly IFleetImageService _imageService;
    private readonly ITenantContext _tenantContext;
    private readonly ITenantRepository _tenantRepository;

    public VehiclesController(
        IVehicleService service,
        IFleetImageService imageService,
        ITenantContext tenantContext,
        ITenantRepository tenantRepository)
    {
        _service = service;
        _imageService = imageService;
        _tenantContext = tenantContext;
        _tenantRepository = tenantRepository;
    }

    // ── VEHICLE ENDPOINTS ────────────────────────────────────────────────

    [HttpPost]
    [Authorize(Policy = "FleetOwnerOrAdmin")]
    [ProducesResponseType(typeof(VehicleResponseDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateVehicleDto dto)
    {
        var userId = Guid.Parse(User.Identity?.Name ?? Guid.Empty.ToString());
        var res = await _service.CreateAsync(dto, userId);
        return res.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = res.Value.Id }, res.Value)
            : BadRequest(res.Error);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(VehicleResponseDto), 200)]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromQuery] string? tenantSlug,
        CancellationToken ct)
    {
        var err = await PublicTenantResolver.TryResolveForPublicDataAsync(
            this, _tenantContext, _tenantRepository, tenantSlug, ct);
        if (err is not null) return err;

        var res = await _service.GetByIdAsync(id);
        return res.IsSuccess ? Ok(res.Value) : NotFound(res.Error);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<VehicleResponseDto>), 200)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] string? tenantSlug,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var err = await PublicTenantResolver.TryResolveForPublicDataAsync(
            this, _tenantContext, _tenantRepository, tenantSlug, ct);
        if (err is not null) return err;

        var res = await _service.GetPagedAsync(page, pageSize);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "FleetOwnerOrAdmin")]
    [ProducesResponseType(typeof(VehicleResponseDto), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVehicleDto dto)
    {
        var userId = Guid.Parse(User.Identity?.Name ?? Guid.Empty.ToString());
        var res = await _service.UpdateAsync(id, dto, userId);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<VehicleResponseDto>), 200)]
    public async Task<IActionResult> Search(
        [FromQuery] string? tenantSlug,
        [FromQuery] VehicleFilterDto filter,
        CancellationToken ct)
    {
        var err = await PublicTenantResolver.TryResolveForPublicDataAsync(
            this, _tenantContext, _tenantRepository, tenantSlug, ct);
        if (err is not null) return err;

        var res = await _service.GetFilteredAsync(filter, ct);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    // ── FLEET IMAGE ENDPOINTS ────────────────────────────────────────────

    /// <summary>
    /// Upload an image for a vehicle
    /// POST /api/vehicles/{vehicleId}/images
    /// </summary>
    [HttpPost("{vehicleId}/images")]
    [Authorize(Policy = "FleetOwnerOrAdmin")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(FleetImageResponseDto), 201)]
    public async Task<IActionResult> UploadFleetImage(
        Guid vehicleId,
        [FromForm] CreateFleetImageDto dto,
        CancellationToken ct)
    {
        var file = dto.File;

        if (file is null || file.Length == 0)
            return BadRequest(new { error = "No file provided." });

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new { error = "File exceeds 5MB limit." });

        var allowed = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
        if (!allowed.Contains(file.ContentType.ToLowerInvariant()))
            return BadRequest(new { error = "Only JPG, PNG, WEBP, and GIF images are allowed." });

        var userId = Guid.Parse(User.Identity?.Name ?? Guid.Empty.ToString());
        var res = await _imageService.UploadImageAsync(vehicleId, file, userId, ct);

        return res.IsSuccess
            ? CreatedAtAction(nameof(GetVehicleImages), new { vehicleId }, res.Value)
            : BadRequest(new { error = res.Error });
    }

    /// <summary>
    /// Get all images for a vehicle
    /// GET /api/vehicles/{vehicleId}/images
    /// </summary>
    [HttpGet("{vehicleId}/images")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(FleetImageResponseDto[]), 200)]
    public async Task<IActionResult> GetVehicleImages(
        Guid vehicleId,
        [FromQuery] string? tenantSlug,
        CancellationToken ct)
    {
        var err = await PublicTenantResolver.TryResolveForPublicDataAsync(
            this, _tenantContext, _tenantRepository, tenantSlug, ct);
        if (err is not null) return err;

        var res = await _imageService.GetFleetImagesAsync(vehicleId, ct);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(new { error = res.Error });
    }

    /// <summary>
    /// Delete a vehicle image
    /// DELETE /api/vehicles/images/{imageId}
    /// </summary>
    [HttpDelete("images/{imageId}")]
    [Authorize(Policy = "FleetOwnerOrAdmin")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> DeleteImage(int imageId, CancellationToken ct)
    {
        var userId = Guid.Parse(User.Identity?.Name ?? Guid.Empty.ToString());
        var res = await _imageService.DeleteImageAsync(imageId, userId, ct);

        return res.IsSuccess
            ? Ok(new { message = res.Value })
            : BadRequest(new { error = res.Error });
    }
}