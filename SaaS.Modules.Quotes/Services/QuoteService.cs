using AutoMapper;
using SaaS.Modules.Listings.Entities;
using SaaS.Modules.Listings.Repositories;
using SaaS.Modules.Quotes.DTOs;
using SaaS.Modules.Quotes.Entities;
using SaaS.Modules.Quotes.Repositories;
using SaaS.Shared;

namespace SaaS.Modules.Quotes.Services;

public sealed class QuoteService(
    IQuoteRepository quoteRepository,
    IListingRepository listingRepository,
    IMapper mapper) : IQuoteService
{
    public async Task<Result<QuoteResponseDto>> SubmitAsync(
        SubmitQuoteDto dto, Guid transporterId, CancellationToken ct = default)
    {
        // ── 1. Validate listing exists and is active ───────────────────────────
        // Global query filter ensures listing belongs to the same tenant.
        var listing = await listingRepository.GetByIdAsync(dto.ListingId, ct);

        if (listing is null)
            return Result<QuoteResponseDto>.Failure("Listing not found.");

        if (listing.Status != ListingStatus.Active)
            return Result<QuoteResponseDto>.Failure(
                "Quotes can only be submitted for active listings.");

        if (listing.ExpiresAt.HasValue && listing.ExpiresAt.Value < DateTime.UtcNow)
            return Result<QuoteResponseDto>.Failure(
                "This listing has expired and no longer accepts quotes.");

        // ── 2. Prevent the listing owner from quoting their own listing ────────
        if (listing.UserId == transporterId)
            return Result<QuoteResponseDto>.Failure(
                "You cannot submit a quote on your own listing.");

        // ── 3. Prevent duplicate pending quotes from the same transporter ──────
        var hasPending = await quoteRepository.HasPendingQuoteAsync(
            dto.ListingId, transporterId, ct);

        if (hasPending)
            return Result<QuoteResponseDto>.Failure(
                "You already have a pending quote for this listing. " +
                "Withdraw it before submitting a new one.");

        // ── 4. Validate expiry ─────────────────────────────────────────────────
        if (dto.ValidUntil.HasValue && dto.ValidUntil.Value <= DateTime.UtcNow)
            return Result<QuoteResponseDto>.Failure(
                "ValidUntil must be a future date.");

        // ── 5. Build and persist ───────────────────────────────────────────────
        var quote = mapper.Map<Quote>(dto);
        quote.TransporterId = transporterId;
        // TenantId auto-stamped by AppDbContext.SaveChangesAsync

        await quoteRepository.AddAsync(quote, ct);
        await quoteRepository.SaveChangesAsync(ct);

        return Result<QuoteResponseDto>.Success(
            mapper.Map<QuoteResponseDto>(quote));
    }

    public async Task<Result<QuoteResponseDto>> GetByIdAsync(
        Guid id, CancellationToken ct = default)
    {
        var quote = await quoteRepository.GetByIdAsync(id, ct);

        return quote is null
            ? Result<QuoteResponseDto>.Failure("Quote not found.")
            : Result<QuoteResponseDto>.Success(mapper.Map<QuoteResponseDto>(quote));
    }

    public async Task<Result<PagedResult<QuoteResponseDto>>> GetByFilterAsync(
        QuoteFilterDto filter, CancellationToken ct = default)
    {
        var paged = await quoteRepository.GetByFilterAsync(filter, ct);

        return Result<PagedResult<QuoteResponseDto>>.Success(
            new PagedResult<QuoteResponseDto>(
                paged.Items.Select(mapper.Map<QuoteResponseDto>).ToList(),
                paged.TotalCount,
                paged.Page,
                paged.PageSize));
    }

    public async Task<Result<QuoteResponseDto>> AcceptAsync(
        Guid id, Guid requestingUserId, AcceptQuoteDto dto, CancellationToken ct = default)
    {
        var quote = await quoteRepository.GetByIdAsync(id, ct);
        if (quote is null)
            return Result<QuoteResponseDto>.Failure("Quote not found.");

        // ── Verify the requesting user owns the listing ────────────────────────
        var listing = await listingRepository.GetByIdAsync(quote.ListingId, ct);
        if (listing is null)
            return Result<QuoteResponseDto>.Failure("Associated listing not found.");

        if (listing.UserId != requestingUserId)
            return Result<QuoteResponseDto>.Failure(
                "Only the listing owner can accept quotes.");

        // ── Validate quote is still pending ───────────────────────────────────
        if (quote.Status != QuoteStatus.Pending)
            return Result<QuoteResponseDto>.Failure(
                $"Only pending quotes can be accepted. Current status: {quote.Status}.");

        // ── Prevent accepting if another quote is already accepted ─────────────
        var alreadyAccepted = await quoteRepository
            .ListingHasAcceptedQuoteAsync(quote.ListingId, ct);

        if (alreadyAccepted)
            return Result<QuoteResponseDto>.Failure(
                "A quote for this listing has already been accepted.");

        // ── Accept ─────────────────────────────────────────────────────────────
        quote.Status = QuoteStatus.Accepted;
        quote.AcceptedAt = DateTime.UtcNow;

        // Auto-pause the listing so no new quotes arrive
        listing.Status = ListingStatus.Paused;

        await quoteRepository.SaveChangesAsync(ct);

        return Result<QuoteResponseDto>.Success(
            mapper.Map<QuoteResponseDto>(quote));
    }

    public async Task<Result<QuoteResponseDto>> RejectAsync(
        Guid id, Guid requestingUserId, RejectQuoteDto dto, CancellationToken ct = default)
    {
        var quote = await quoteRepository.GetByIdAsync(id, ct);
        if (quote is null)
            return Result<QuoteResponseDto>.Failure("Quote not found.");

        // ── Verify the requesting user owns the listing ────────────────────────
        var listing = await listingRepository.GetByIdAsync(quote.ListingId, ct);
        if (listing is null)
            return Result<QuoteResponseDto>.Failure("Associated listing not found.");

        if (listing.UserId != requestingUserId)
            return Result<QuoteResponseDto>.Failure(
                "Only the listing owner can reject quotes.");

        if (quote.Status != QuoteStatus.Pending)
            return Result<QuoteResponseDto>.Failure(
                $"Only pending quotes can be rejected. Current status: {quote.Status}.");

        // ── Reject ─────────────────────────────────────────────────────────────
        quote.Status = QuoteStatus.Rejected;
        quote.RejectionReason = dto.Reason;
        quote.RejectedAt = DateTime.UtcNow;

        await quoteRepository.SaveChangesAsync(ct);

        return Result<QuoteResponseDto>.Success(
            mapper.Map<QuoteResponseDto>(quote));
    }

    public async Task<Result<QuoteResponseDto>> WithdrawAsync(
        Guid id, Guid transporterId, WithdrawQuoteDto dto, CancellationToken ct = default)
    {
        var quote = await quoteRepository.GetByIdAsync(id, ct);
        if (quote is null)
            return Result<QuoteResponseDto>.Failure("Quote not found.");

        // ── Only the transporter who submitted can withdraw ────────────────────
        if (quote.TransporterId != transporterId)
            return Result<QuoteResponseDto>.Failure(
                "You can only withdraw your own quotes.");

        if (quote.Status != QuoteStatus.Pending)
            return Result<QuoteResponseDto>.Failure(
                $"Only pending quotes can be withdrawn. Current status: {quote.Status}.");

        // ── Withdraw ───────────────────────────────────────────────────────────
        quote.Status = QuoteStatus.Withdrawn;
        quote.WithdrawnAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(dto.Reason))
            quote.RejectionReason = dto.Reason;

        await quoteRepository.SaveChangesAsync(ct);

        return Result<QuoteResponseDto>.Success(
            mapper.Map<QuoteResponseDto>(quote));
    }
}