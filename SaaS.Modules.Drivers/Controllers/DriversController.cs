using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Drivers.DTOs;
using SaaS.Modules.Drivers.Services;
using SaaS.Shared;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SaaS.Modules.Drivers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class DriversController : ControllerBase
    {
        private readonly IDriverService _service;

        public DriversController(IDriverService service) => _service = service;

        // ── GET /api/drivers?page=1&pageSize=12&status=Active&region=riyadh ──────

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(PagedResult<DriverResponseDto>), 200)]
        public async Task<IActionResult> GetAll(
            [FromQuery] DriverFilterDto filter,
            CancellationToken ct)
        {
            var res = await _service.GetFilteredAsync(filter, ct);
            return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
        }

        // ── GET /api/drivers/{id} ─────────────────────────────────────────────────

        [HttpGet("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(DriverResponseDto), 200)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var res = await _service.GetByIdAsync(id, ct);
            return res.IsSuccess ? Ok(res.Value) : NotFound(res.Error);
        }

        // ── POST /api/drivers ─────────────────────────────────────────────────────

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(DriverResponseDto), 201)]
        public async Task<IActionResult> Create(
            [FromBody] CreateDriverDto dto,
            CancellationToken ct)
        {
            var userId = Guid.Parse(User.Identity?.Name ?? Guid.Empty.ToString());
            var res = await _service.CreateAsync(dto, userId, ct);
            return res.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = res.Value!.Id }, res.Value)
                : BadRequest(new { error = res.Error });
        }

        // ── PUT /api/drivers/{id} ─────────────────────────────────────────────────

        [HttpPut("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(DriverResponseDto), 200)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateDriverDto dto,
            CancellationToken ct)
        {
            var userId = Guid.Parse(User.Identity?.Name ?? Guid.Empty.ToString());
            var res = await _service.UpdateAsync(id, dto, userId, ct);
            return res.IsSuccess ? Ok(res.Value) : BadRequest(new { error = res.Error });
        }

        // ── DELETE /api/drivers/{id} ──────────────────────────────────────────────

        [HttpDelete("{id:guid}")]
        [Authorize]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var userId = Guid.Parse(User.Identity?.Name ?? Guid.Empty.ToString());
            var res = await _service.DeleteAsync(id, userId, ct);
            return res.IsSuccess ? Ok(new { message = res.Value }) : BadRequest(new { error = res.Error });
        }
    }
}
