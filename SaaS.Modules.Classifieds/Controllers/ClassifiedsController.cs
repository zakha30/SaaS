using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Classifieds.DTOs;
using SaaS.Modules.Classifieds.Services;

namespace SaaS.Modules.Classifieds.Controllers;

[ApiController]
[Route("api/classifieds")]
public sealed class ClassifiedsController : ControllerBase
{
    private readonly IClassifiedService service;
    public ClassifiedsController(IClassifiedService service) => this.service = service;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] ClassifiedFilterDto filter, CancellationToken ct)
    {
        var paged = await service.GetFilteredAsync(filter, ct);
        return Ok(paged);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClassifiedDto dto, CancellationToken ct)
    {
        var created = await service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(System.Guid id, CancellationToken ct)
    {
        return NotFound();
    }
}
