using System.ComponentModel.DataAnnotations;
using SaaS.Modules.Quotes.Entities;

namespace SaaS.Modules.Quotes.DTOs;

// ── Submit ────────────────────────────────────────────────────────────────────

public sealed record SubmitQuoteDto(
    [Required] Guid ListingId,
    [Required, Range(0, double.MaxValue)] decimal Price,
    [Required, MinLength(5), MaxLength(1000)] string Message,
    [MaxLength(3)] string Currency = "USD",
                                          DateTime? ValidUntil = null);

// ── Accept ────────────────────────────────────────────────────────────────────

public sealed record AcceptQuoteDto(
    [MaxLength(500)] string? Message = null);

// ── Reject ────────────────────────────────────────────────────────────────────

public sealed record RejectQuoteDto(
    [Required, MinLength(3), MaxLength(500)] string Reason);

// ── Withdraw ──────────────────────────────────────────────────────────────────

public sealed record WithdrawQuoteDto(
    [MaxLength(500)] string? Reason = null);

// ── Response ──────────────────────────────────────────────────────────────────

public sealed record QuoteResponseDto(
    Guid Id,
    Guid TenantId,
    Guid ListingId,
    Guid TransporterId,
    decimal Price,
    string Currency,
    string Message,
    QuoteStatus Status,
    string StatusLabel,
    string? RejectionReason,
    DateTime? ValidUntil,
    DateTime? AcceptedAt,
    DateTime? RejectedAt,
    DateTime? WithdrawnAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

// ── List filter ───────────────────────────────────────────────────────────────

public sealed record QuoteFilterDto(
    Guid? ListingId = null,
    Guid? TransporterId = null,
    QuoteStatus? Status = null,
    int Page = 1,
    int PageSize = 20);