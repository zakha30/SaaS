using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Loads.DTOs;
using SaaS.Modules.Loads.Services;

namespace SaaS.Modules.Loads.Controllers;

[ApiController]
[Route("api/loads")]
public sealed class AvailableLoadsController : ControllerBase
{
    private readonly IAvailableLoadService service;
    public AvailableLoadsController(IAvailableLoadService service) => this.service = service;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] LoadFilterDto filter, CancellationToken ct)
    {
        var paged = await service.GetFilteredAsync(filter, ct);
        return Ok(paged);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLoadDto dto, CancellationToken ct)
    {
        var created = await service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(System.Guid id, CancellationToken ct)
    {
        // repo/service method not implemented for GetById; returning NotFound for now
        return NotFound();
    }
}
