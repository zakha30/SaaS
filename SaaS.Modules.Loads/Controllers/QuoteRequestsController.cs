using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Loads.DTOs;
using SaaS.Modules.Loads.Services;

namespace SaaS.Modules.Loads.Controllers;

[ApiController]
[Route("api/quote-requests")]
public sealed class QuoteRequestsController : ControllerBase
{
    private readonly IQuoteRequestService service;
    public QuoteRequestsController(IQuoteRequestService service) => this.service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuoteRequestDto dto, CancellationToken ct)
    {
        var created = await service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitQuote([FromBody] QuoteSubmissionDto dto, CancellationToken ct)
    {
        await service.SubmitQuoteAsync(dto, ct);
        return Accepted();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(System.Guid id, CancellationToken ct)
    {
        return NotFound();
    }
}
