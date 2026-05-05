using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Loads.DTOs;
using SaaS.Modules.Loads.Services;

namespace SaaS.Modules.Loads.Controllers;

[ApiController]
[Route("api/quote-requests")]
public sealed class QuoteRequestsController(IQuoteRequestService service) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "ShipperOrAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateQuoteRequestDto dto, CancellationToken ct)
    {
        var created = await service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("submit")]
    [Authorize(Policy = "TransporterOrAdmin")]
    public async Task<IActionResult> SubmitQuote([FromBody] QuoteSubmissionDto dto, CancellationToken ct)
    {
        var userId = Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User identity claim missing."));
        await service.SubmitQuoteAsync(dto, userId, ct);
        return Accepted();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        _ = id;
        return NotFound();
    }
}
