using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Jobs.DTOs;
using SaaS.Modules.Jobs.Services;

namespace SaaS.Modules.Jobs.Controllers;

[ApiController]
[Route("api/jobs")]
public sealed class JobsController : ControllerBase
{
    private readonly IJobService service;
    public JobsController(IJobService service) => this.service = service;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] JobFilterDto filter, CancellationToken ct)
    {
        var paged = await service.GetFilteredAsync(filter, ct);
        return Ok(paged);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateJobDto dto, CancellationToken ct)
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
