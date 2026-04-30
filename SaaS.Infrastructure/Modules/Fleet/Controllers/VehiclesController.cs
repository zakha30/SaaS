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

    [HttpPost("images")]
    [Authorize]
    public async Task<IActionResult> UploadImage(
    IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file provided.");

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("File exceeds 5MB limit.");

        var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!allowed.Contains(file.ContentType.ToLower()))
            return BadRequest("Only JPG, PNG and WEBP images are allowed.");

        var res = await service.UploadImageAsync(file, ct);
        return res.IsSuccess ? Ok(new { url = res.Value }) : BadRequest(res.Error);
    }


    /// <summary>
    /// Upload an image for a fleet (vehicle)
    /// </summary>
    /// <param name="fleetId">The Fleet/Vehicle ID</param>
    /// <param name="file">The image file (JPG, PNG, WEBP, GIF - max 5MB)</param>
    /// <param name="ct">Cancellation token</param>
    [HttpPost("{fleetId}/images")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage(
        Guid fleetId,
        [FromForm] IFormFile file,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "No file provided." });

        var userId = User.Identity?.Name ?? string.Empty;
        var res = await _imageService.UploadImageAsync(fleetId, file, userId, ct);

        return res.IsSuccess
            ? CreatedAtAction(nameof(GetFleetImages), new { fleetId }, res.Value)
            : BadRequest(new { error = res.Error });
    }

    /// <summary>
    /// Get all images for a fleet (vehicle)
    /// </summary>
    /// <param name="fleetId">The Fleet/Vehicle ID</param>
    /// <param name="ct">Cancellation token</param>
    [HttpGet("{fleetId}/images")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFleetImages(Guid fleetId, CancellationToken ct)
    {
        var res = await _imageService.GetFleetImagesAsync(fleetId, ct);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(new { error = res.Error });
    }

    /// <summary>
    /// Delete a specific image
    /// </summary>
    /// <param name="imageId">The Image ID to delete</param>
    /// <param name="ct">Cancellation token</param>
    [HttpDelete("images/{imageId}")]
    [Authorize]
    public async Task<IActionResult> DeleteImage(int imageId, CancellationToken ct)
    {
        var userId = Guid.Parse(User.Identity?.Name ?? Guid.Empty.ToString());
        var res = await _imageService.DeleteImageAsync(imageId, userId, ct);

        return res.IsSuccess
            ? Ok(new { message = res.Value })
            : BadRequest(new { error = res.Error });
    }
}
