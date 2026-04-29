using SaaS.Modules.Listings.Entities;
using SaaS.Modules.Quotes.Entities;

namespace SaaS.Modules.Dashboard.DTOs;

// ── Summary counts shown at the top of the dashboard ─────────────────────────

public sealed record DashboardSummaryDto(
    int TotalListings,
    int ActiveListings,
    int TotalQuotesSubmitted,
    int PendingQuotesSubmitted,
    int TotalQuotesReceived,
    int PendingQuotesReceived,
    int AcceptedQuotesReceived,
    decimal TotalRevenueFromAcceptedQuotes);

// ── My Listings ───────────────────────────────────────────────────────────────

public sealed record MyListingDto(
    Guid Id,
    string Title,
    string LocationFrom,
    string LocationTo,
    ListingType Type,
    decimal Price,
    string Currency,
    ListingStatus Status,
    int TotalQuotes,       // quotes received on this listing
    int PendingQuotes,     // quotes awaiting decision
    bool HasAcceptedQuote,
    DateTime? ExpiresAt,
    DateTime CreatedAt);

public sealed record MyListingsResponseDto(
    IReadOnlyList<MyListingDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);

// ── My Quotes (submitted by me as transporter) ────────────────────────────────

public sealed record MyQuoteDto(
    Guid Id,
    Guid ListingId,
    string ListingTitle,
    string LocationFrom,
    string LocationTo,
    decimal MyPrice,
    string Currency,
    string Message,
    QuoteStatus Status,
    string? RejectionReason,
    DateTime? ValidUntil,
    DateTime CreatedAt);

public sealed record MyQuotesResponseDto(
    IReadOnlyList<MyQuoteDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);

// ── Quotes received on my listings ────────────────────────────────────────────

public sealed record ReceivedQuoteDto(
    Guid Id,
    Guid ListingId,
    string ListingTitle,
    Guid TransporterId,
    string TransporterName,
    string TransporterEmail,
    decimal Price,
    string Currency,
    string Message,
    QuoteStatus Status,
    string? RejectionReason,
    DateTime? ValidUntil,
    DateTime? AcceptedAt,
    DateTime? RejectedAt,
    DateTime CreatedAt);

public sealed record ReceivedQuotesResponseDto(
    IReadOnlyList<ReceivedQuoteDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);

// ── Query filters ─────────────────────────────────────────────────────────────

public sealed record MyListingsFilterDto(
    ListingStatus? Status = null,
    int Page = 1,
    int PageSize = 20);

public sealed record MyQuotesFilterDto(
    QuoteStatus? Status = null,
    int Page = 1,
    int PageSize = 20);

public sealed record ReceivedQuotesFilterDto(
    Guid? ListingId = null,
    QuoteStatus? Status = null,
    int Page = 1,
    int PageSize = 20);