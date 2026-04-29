using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Forum.DTOs;
using SaaS.Modules.Forum.Services;

namespace SaaS.Modules.Forum.Controllers;

[ApiController]
[Route("api/forum")]
public sealed class ForumController : ControllerBase
{
    private readonly IThreadService service;
    public ForumController(IThreadService service) => this.service = service;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] ThreadFilterDto filter, CancellationToken ct)
    {
        var paged = await service.GetFilteredAsync(filter, ct);
        return Ok(paged);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateThreadDto dto, CancellationToken ct)
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
