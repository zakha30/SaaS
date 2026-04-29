using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Data;
using SaaS.Modules.Quotes.DTOs;
using SaaS.Modules.Quotes.Entities;
using SaaS.Modules.Quotes.Repositories;
using SaaS.Shared;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaaS.Infrastructure.Repositories;

public sealed class QuoteRepository(AppDbContext db) : IQuoteRepository
{
    // Global query filter on AppDbContext automatically scopes
    // every query to the current tenant — no manual TenantId filtering needed.

    public async Task<Quote?> GetByIdAsync(
        Guid id, CancellationToken ct = default) =>
        await db.Quotes
                .FirstOrDefaultAsync(q => q.Id == id, ct);

    public async Task<PagedResult<Quote>> GetByFilterAsync(
        QuoteFilterDto filter, CancellationToken ct = default)
    {
        var query = db.Quotes.AsNoTracking();

        if (filter.ListingId.HasValue)
            query = query.Where(q => q.ListingId == filter.ListingId.Value);

        if (filter.TransporterId.HasValue)
            query = query.Where(q => q.TransporterId == filter.TransporterId.Value);

        if (filter.Status.HasValue)
            query = query.Where(q => q.Status == filter.Status.Value);

        query = query.OrderByDescending(q => q.CreatedAt);

        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Quote>(items, total, filter.Page, filter.PageSize);
    }

    public async Task<bool> HasPendingQuoteAsync(
        Guid listingId, Guid transporterId, CancellationToken ct = default) =>
        await db.Quotes.AnyAsync(q =>
            q.ListingId == listingId &&
            q.TransporterId == transporterId &&
            q.Status == QuoteStatus.Pending, ct);

    public async Task<bool> ListingHasAcceptedQuoteAsync(
        Guid listingId, CancellationToken ct = default) =>
        await db.Quotes.AnyAsync(q =>
            q.ListingId == listingId &&
            q.Status == QuoteStatus.Accepted, ct);

    public async Task AddAsync(
        Quote quote, CancellationToken ct = default) =>
        await db.Quotes.AddAsync(quote, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);
}