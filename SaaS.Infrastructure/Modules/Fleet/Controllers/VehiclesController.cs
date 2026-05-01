using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaaS.Infrastructure.Modules.Fleet.DTOs;
using SaaS.Infrastructure.Modules.Fleet.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SaaS.Infrastructure.Modules.Fleet.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class VehiclesController : ControllerBase
{
    private readonly IVehicleService service;
    private readonly IFleetImageService _imageService;

    public VehiclesController(IVehicleService service, IFleetImageService imageService)
    {
        this.service = service;
        _imageService = imageService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateVehicleDto dto)
    {
        var userId = Guid.Parse(User.Identity?.Name ?? Guid.Empty.ToString());
        var res = await service.CreateAsync(dto, userId);
        return res.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = res.Value.Id }, res.Value) : BadRequest(res.Error);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var res = await service.GetByIdAsync(id);
        return res.IsSuccess ? Ok(res.Value) : NotFound(res.Error);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var res = await service.GetPagedAsync(page, pageSize);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVehicleDto dto)
    {
        var userId = Guid.Parse(User.Identity?.Name ?? Guid.Empty.ToString());
        var res = await service.UpdateAsync(id, dto, userId);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromQuery] VehicleFilterDto filter, CancellationToken ct)
    {
        var res = await service.GetFilteredAsync(filter, ct);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }


    /// <summary>
    /// Upload an image for a fleet (vehicle)
    /// </summary>
    [HttpPost("{vehicleId}/images")]  // ← renamed parameter
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadFleetImage(
        Guid vehicleId,  // ← renamed
        [FromForm] IFormFile file,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "No file provided." });

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new { error = "File exceeds 5MB limit." });

        var allowed = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
        if (!allowed.Contains(file.ContentType.ToLowerInvariant()))
            return BadRequest(new { error = "Only JPG, PNG, WEBP, and GIF images are allowed." });

        var userId = Guid.Parse(User.Identity?.Name ?? Guid.Empty.ToString());
        var res = await _imageService.UploadImageAsync(vehicleId, file, userId, ct);  // ← pass vehicleId

        return res.IsSuccess
            ? CreatedAtAction(nameof(GetFleetImages), new { vehicleId }, res.Value)  // ← updated
            : BadRequest(new { error = res.Error });
    }

    /// <summary>
    /// Get all images for a vehicle
    /// </summary>
    [HttpGet("{vehicleId}/images")]  // ← renamed
    [AllowAnonymous]
    public async Task<IActionResult> GetFleetImages(Guid vehicleId, CancellationToken ct)  // ← renamed
    {
        var res = await _imageService.GetFleetImagesAsync(vehicleId, ct);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(new { error = res.Error });
    }

    /// <summary>
    /// Delete a specific image
    /// </summary>
    [HttpDelete("images/{imageId}")]
    [Authorize]
    public async Task<IActionResult> DeleteImage(int imageId, CancellationToken ct)  // ← int instead of Guid
    {
        var userId = Guid.Parse(User.Identity?.Name ?? Guid.Empty.ToString());
        var res = await _imageService.DeleteImageAsync(imageId, userId, ct);

        return res.IsSuccess
            ? Ok(new { message = res.Value })
            : BadRequest(new { error = res.Error });
    }
}
