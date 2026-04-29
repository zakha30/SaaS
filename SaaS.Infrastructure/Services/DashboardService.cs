using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Data;
using SaaS.Modules.Dashboard.DTOs;
using SaaS.Modules.Dashboard.Services;
using SaaS.Modules.Listings.Entities;
using SaaS.Modules.Quotes.Entities;
using SaaS.Shared;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaaS.Infrastructure.Services;

public sealed class DashboardService(AppDbContext db) : IDashboardService
{
    // ── Summary ────────────────────────────────────────────────────────────────

    public async Task<Result<DashboardSummaryDto>> GetSummaryAsync(
        Guid userId, CancellationToken ct = default)
    {
        // All queries below benefit from the global query filter:
        // WHERE TenantId = @currentTenantId AND IsDeleted = 0

        // Listings stats — single query with projections
        var listingStats = await db.Listings
            .Where(l => l.UserId == userId)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Active = g.Count(l => l.Status == ListingStatus.Active)
            })
            .FirstOrDefaultAsync(ct);

        // Quotes submitted by me (as transporter)
        var submittedStats = await db.Quotes
            .Where(q => q.TransporterId == userId)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Pending = g.Count(q => q.Status == QuoteStatus.Pending)
            })
            .FirstOrDefaultAsync(ct);

        // Quotes received on my listings — join in SQL, not in memory
        var receivedStats = await db.Quotes
            .Join(db.Listings,
                q => q.ListingId,
                l => l.Id,
                (q, l) => new { Quote = q, Listing = l })
            .Where(x => x.Listing.UserId == userId)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Pending = g.Count(x => x.Quote.Status == QuoteStatus.Pending),
                Accepted = g.Count(x => x.Quote.Status == QuoteStatus.Accepted),
                Revenue = g
                    .Where(x => x.Quote.Status == QuoteStatus.Accepted)
                    .Sum(x => (decimal?)x.Quote.Price) ?? 0m
            })
            .FirstOrDefaultAsync(ct);

        var summary = new DashboardSummaryDto(
            TotalListings: listingStats?.Total ?? 0,
            ActiveListings: listingStats?.Active ?? 0,
            TotalQuotesSubmitted: submittedStats?.Total ?? 0,
            PendingQuotesSubmitted: submittedStats?.Pending ?? 0,
            TotalQuotesReceived: receivedStats?.Total ?? 0,
            PendingQuotesReceived: receivedStats?.Pending ?? 0,
            AcceptedQuotesReceived: receivedStats?.Accepted ?? 0,
            TotalRevenueFromAcceptedQuotes: receivedStats?.Revenue ?? 0m);

        return Result<DashboardSummaryDto>.Success(summary);
    }

    // ── My Listings ────────────────────────────────────────────────────────────

    public async Task<Result<MyListingsResponseDto>> GetMyListingsAsync(
        Guid userId, MyListingsFilterDto filter, CancellationToken ct = default)
    {
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        // Base query scoped to current user's listings
        var listingQuery = db.Listings
            .AsNoTracking()
            .Where(l => l.UserId == userId);

        if (filter.Status.HasValue)
            listingQuery = listingQuery.Where(l => l.Status == filter.Status.Value);

        var total = await listingQuery.CountAsync(ct);

        // Left join with quotes to get per-listing counts in a single SQL query.
        // GroupJoin produces LEFT OUTER JOIN — listings with zero quotes are included.
        var items = await listingQuery
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .GroupJoin(
                db.Quotes.AsNoTracking(),
                l => l.Id,
                q => q.ListingId,
                (listing, quotes) => new MyListingDto(
                    listing.Id,
                    listing.Title,
                    listing.LocationFrom,
                    listing.LocationTo,
                    listing.Type,
                    listing.Price,
                    listing.Currency,
                    listing.Status,
                    TotalQuotes: quotes.Count(),
                    PendingQuotes: quotes.Count(q => q.Status == QuoteStatus.Pending),
                    HasAcceptedQuote: quotes.Any(q => q.Status == QuoteStatus.Accepted),
                    listing.ExpiresAt,
                    listing.CreatedAt))
            .ToListAsync(ct);

        var pages = (int)Math.Ceiling(total / (double)pageSize);

        return Result<MyListingsResponseDto>.Success(
            new MyListingsResponseDto(
                Items: items,
                TotalCount: total,
                Page: page,
                PageSize: pageSize,
                TotalPages: pages));
    }

    // ── My Quotes (submitted by me as transporter) ─────────────────────────────

    public async Task<Result<MyQuotesResponseDto>> GetMyQuotesAsync(
        Guid userId, MyQuotesFilterDto filter, CancellationToken ct = default)
    {
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        var quoteQuery = db.Quotes
            .AsNoTracking()
            .Where(q => q.TransporterId == userId);

        if (filter.Status.HasValue)
            quoteQuery = quoteQuery.Where(q => q.Status == filter.Status.Value);

        var total = await quoteQuery.CountAsync(ct);

        // Inner join with Listings to get listing details alongside each quote.
        // Both sides are tenant-filtered by the global query filter automatically.
        var items = await quoteQuery
            .OrderByDescending(q => q.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Join(
                db.Listings.AsNoTracking(),
                q => q.ListingId,
                l => l.Id,
                (q, l) => new MyQuoteDto(
                    q.Id,
                    q.ListingId,
                    ListingTitle: l.Title,
                    LocationFrom: l.LocationFrom,
                    LocationTo: l.LocationTo,
                    MyPrice: q.Price,
                    q.Currency,
                    q.Message,
                    q.Status,
                    q.RejectionReason,
                    q.ValidUntil,
                    q.CreatedAt))
            .ToListAsync(ct);

        var pages = (int)Math.Ceiling(total / (double)pageSize);

        return Result<MyQuotesResponseDto>.Success(
            new MyQuotesResponseDto(
                Items: items,
                TotalCount: total,
                Page: page,
                PageSize: pageSize,
                TotalPages: pages));
    }

    // ── Quotes received on my listings ─────────────────────────────────────────

    public async Task<Result<ReceivedQuotesResponseDto>> GetReceivedQuotesAsync(
        Guid userId, ReceivedQuotesFilterDto filter, CancellationToken ct = default)
    {
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        // Start from listings owned by the current user,
        // then join quotes and transporter info in one query.
        var baseQuery =
            from listing in db.Listings.AsNoTracking()
            where listing.UserId == userId
            join quote in db.Quotes.AsNoTracking()
                on listing.Id equals quote.ListingId
            join transporter in db.Users.AsNoTracking()
                on quote.TransporterId equals transporter.Id
            select new { listing, quote, transporter };

        // Optional filters
        if (filter.ListingId.HasValue)
            baseQuery = baseQuery.Where(x => x.listing.Id == filter.ListingId.Value);

        if (filter.Status.HasValue)
            baseQuery = baseQuery.Where(x => x.quote.Status == filter.Status.Value);

        var total = await baseQuery.CountAsync(ct);

        var items = await baseQuery
            .OrderByDescending(x => x.quote.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ReceivedQuoteDto(
                x.quote.Id,
                x.listing.Id,
                ListingTitle: x.listing.Title,
                TransporterId: x.transporter.Id,
                TransporterName: x.transporter.FirstName + " " + x.transporter.LastName,
                TransporterEmail: x.transporter.Email,
                Price: x.quote.Price,
                x.quote.Currency,
                x.quote.Message,
                x.quote.Status,
                x.quote.RejectionReason,
                x.quote.ValidUntil,
                x.quote.AcceptedAt,
                x.quote.RejectedAt,
                x.quote.CreatedAt))
            .ToListAsync(ct);

        var pages = (int)Math.Ceiling(total / (double)pageSize);

        return Result<ReceivedQuotesResponseDto>.Success(
            new ReceivedQuotesResponseDto(
                Items: items,
                TotalCount: total,
                Page: page,
                PageSize: pageSize,
                TotalPages: pages));
    }
}