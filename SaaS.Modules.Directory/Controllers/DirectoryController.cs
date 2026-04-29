using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Directory.DTOs;
using SaaS.Modules.Directory.Services;

namespace SaaS.Modules.Directory.Controllers;

[ApiController]
[Route("api/directory")]
public sealed class DirectoryController : ControllerBase
{
    private readonly IDirectoryService service;
    public DirectoryController(IDirectoryService service) => this.service = service;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DirectoryFilterDto filter, CancellationToken ct)
    {
        var paged = await service.GetFilteredAsync(filter, ct);
        return Ok(paged);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDirectoryEntryDto dto, CancellationToken ct)
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
