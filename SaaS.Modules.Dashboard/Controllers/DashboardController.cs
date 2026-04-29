using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Dashboard.DTOs;
using SaaS.Modules.Dashboard.Services;

namespace SaaS.Modules.Dashboard.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public sealed class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    private Guid UserId =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User identity claim missing."));

    // ── Summary ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns aggregated counts for the current user's activity.
    /// Includes listing counts, quote counts, and total revenue.
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken ct)
    {
        var result = await dashboardService.GetSummaryAsync(UserId, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : StatusCode(500, new { error = result.Error });
    }

    // ── My Listings ────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the current user's listings with per-listing quote counts.
    /// Supports status filter and pagination.
    ///
    /// GET /api/dashboard/my-listings?status=Active&page=1&pageSize=20
    /// </summary>
    [HttpGet("my-listings")]
    public async Task<IActionResult> GetMyListings(
        [FromQuery] MyListingsFilterDto filter, CancellationToken ct)
    {
        var result = await dashboardService.GetMyListingsAsync(UserId, filter, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    // ── My Quotes ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns quotes submitted by the current user as a transporter.
    /// Includes listing title and route for context.
    ///
    /// GET /api/dashboard/my-quotes?status=Pending&page=1&pageSize=20
    /// </summary>
    [HttpGet("my-quotes")]
    public async Task<IActionResult> GetMyQuotes(
        [FromQuery] MyQuotesFilterDto filter, CancellationToken ct)
    {
        var result = await dashboardService.GetMyQuotesAsync(UserId, filter, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    // ── Received Quotes ────────────────────────────────────────────────────────

    /// <summary>
    /// Returns all quotes received on the current user's listings.
    /// Includes transporter name and email for direct contact.
    /// Optionally filter by specific listing or quote status.
    ///
    /// GET /api/dashboard/received-quotes?listingId=xxx&status=Pending&page=1
    /// </summary>
    [HttpGet("received-quotes")]
    public async Task<IActionResult> GetReceivedQuotes(
        [FromQuery] ReceivedQuotesFilterDto filter, CancellationToken ct)
    {
        var result = await dashboardService.GetReceivedQuotesAsync(UserId, filter, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }
}