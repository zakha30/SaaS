using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Quotes.DTOs;
using SaaS.Modules.Quotes.Services;

namespace SaaS.Modules.Quotes.Controllers;

[ApiController]
[Route("api/quotes")]
[Authorize]
public sealed class QuotesController(IQuoteService service) : ControllerBase
{
    private Guid UserId =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User identity claim missing."));

    // ── Submit ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Transporter submits a quote (bid) on an active listing.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "TransporterOrAdmin")]
    public async Task<IActionResult> Submit(
        [FromBody] SubmitQuoteDto dto, CancellationToken ct)
    {
        var result = await service.SubmitAsync(dto, UserId, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : BadRequest(new { error = result.Error });
    }

    // ── Read ───────────────────────────────────────────────────────────────────

    /// <summary>
    /// Get a single quote by id.
    /// Global query filter guarantees it belongs to the current tenant.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await service.GetByIdAsync(id, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Get all quotes for a listing (listing owner view).
    /// </summary>
    [HttpGet("listing/{listingId:guid}")]
    public async Task<IActionResult> GetByListing(
        Guid listingId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var filter = new QuoteFilterDto(
            ListingId: listingId,
            Page: page,
            PageSize: pageSize);

        var result = await service.GetByFilterAsync(filter, ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Get all quotes submitted by the current transporter.
    /// </summary>
    [HttpGet("my-quotes")]
    public async Task<IActionResult> GetMyQuotes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var filter = new QuoteFilterDto(
            TransporterId: UserId,
            Page: page,
            PageSize: pageSize);

        var result = await service.GetByFilterAsync(filter, ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Get filtered quotes — supports listing, transporter, and status filters.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetByFilter(
        [FromQuery] QuoteFilterDto filter, CancellationToken ct)
    {
        var result = await service.GetByFilterAsync(filter, ct);
        return Ok(result.Value);
    }

    // ── Actions ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Listing owner accepts a quote. Listing is auto-paused.
    /// </summary>
    [HttpPatch("{id:guid}/accept")]
    public async Task<IActionResult> Accept(
        Guid id, [FromBody] AcceptQuoteDto dto, CancellationToken ct)
    {
        var result = await service.AcceptAsync(id, UserId, dto, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error!.Contains("permission") || result.Error.Contains("owner")
                ? Forbid()
                : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Listing owner rejects a quote with a reason.
    /// </summary>
    [HttpPatch("{id:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid id, [FromBody] RejectQuoteDto dto, CancellationToken ct)
    {
        var result = await service.RejectAsync(id, UserId, dto, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error!.Contains("permission") || result.Error.Contains("owner")
                ? Forbid()
                : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Transporter withdraws their own pending quote.
    /// </summary>
    [HttpPatch("{id:guid}/withdraw")]
    public async Task<IActionResult> Withdraw(
        Guid id, [FromBody] WithdrawQuoteDto dto, CancellationToken ct)
    {
        var result = await service.WithdrawAsync(id, UserId, dto, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error!.Contains("own")
                ? Forbid()
                : BadRequest(new { error = result.Error });
    }
}