using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Data;
using SaaS.Modules.Listings.DTOs;
using SaaS.Modules.Listings.Entities;
using SaaS.Modules.Listings.Repositories;
using SaaS.Shared;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaaS.Infrastructure.Repositories;

public sealed class ListingRepository(AppDbContext db) : IListingRepository
{
    // ── Base query ─────────────────────────────────────────────────────────────
    // The global query filter on AppDbContext automatically appends:
    //   WHERE TenantId = @currentTenantId AND IsDeleted = 0
    // to every query in this class. No manual tenant filtering is ever needed.

    private IQueryable<Listing> BaseQuery =>
        db.Listings.AsNoTracking();

    // ── GetByIdAsync ───────────────────────────────────────────────────────────

    public async Task<Listing?> GetByIdAsync(
        Guid id, CancellationToken ct = default) =>
        await db.Listings
                .FirstOrDefaultAsync(l => l.Id == id, ct);

    // ── GetPagedAsync ──────────────────────────────────────────────────────────

    public async Task<PagedResult<Listing>> GetPagedAsync(
        int page, int pageSize, CancellationToken ct = default)
    {
        var query = BaseQuery
            .OrderByDescending(l => l.CreatedAt);

        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Listing>(items, total, page, pageSize);
    }

    // ── SearchAsync ────────────────────────────────────────────────────────────

    public async Task<PagedResult<Listing>> SearchAsync(
        ListingSearchDto search, CancellationToken ct = default)
    {
        var query = BuildSearchQuery(search);

        // COUNT runs on the filtered query before pagination
        // EF Core generates a single SELECT COUNT(*) query
        var total = await query.CountAsync(ct);

        // Apply sort AFTER count — sort does not affect count result
        query = ApplySorting(query, search.SortBy);

        var items = await query
            .Skip((search.SafePage - 1) * search.SafePageSize)
            .Take(search.SafePageSize)
            .ToListAsync(ct);

        return new PagedResult<Listing>(
            items, total, search.SafePage, search.SafePageSize);
    }

    // ── CountByStatusAsync ─────────────────────────────────────────────────────

    public async Task<int> CountByStatusAsync(
        ListingStatus status, CancellationToken ct = default) =>
        await BaseQuery
            .CountAsync(l => l.Status == status, ct);

    // ── ExistsAsync ────────────────────────────────────────────────────────────

    public async Task<bool> ExistsAsync(
        Guid id, CancellationToken ct = default) =>
        await db.Listings
                .AnyAsync(l => l.Id == id, ct);

    // ── AddAsync ───────────────────────────────────────────────────────────────

    public async Task AddAsync(
        Listing listing, CancellationToken ct = default) =>
        await db.Listings.AddAsync(listing, ct);

    // ── SaveChangesAsync ───────────────────────────────────────────────────────

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);

    // ── Private: Build filtered query ──────────────────────────────────────────

    private IQueryable<Listing> BuildSearchQuery(ListingSearchDto search)
    {
        // Start from the tenant-scoped base query
        var query = BaseQuery;

        // ── Status ─────────────────────────────────────────────────────────────
        // Always filter by status — defaults to Active
        query = query.Where(l => l.Status == search.Status);

        // ── Location from ──────────────────────────────────────────────────────
        // Case-insensitive contains — SQL Server collation handles case
        if (!string.IsNullOrWhiteSpace(search.LocationFrom))
        {
            var locationFrom = search.LocationFrom.Trim();
            query = query.Where(l =>
                l.LocationFrom.Contains(locationFrom));
        }

        // ── Location to ────────────────────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(search.LocationTo))
        {
            var locationTo = search.LocationTo.Trim();
            query = query.Where(l =>
                l.LocationTo.Contains(locationTo));
        }

        // ── Listing type ───────────────────────────────────────────────────────
        if (search.Type.HasValue)
            query = query.Where(l => l.Type == search.Type.Value);

        // ── Keyword — searches Title, Description, CargoType ───────────────────
        // Uses a single OR predicate so EF generates one WHERE clause.
        // EF Core translates this to LIKE on SQL Server.
        if (!string.IsNullOrWhiteSpace(search.Keyword))
        {
            var keyword = search.Keyword.Trim();
            query = query.Where(l =>
                l.Title.Contains(keyword) ||
                (l.Description != null && l.Description.Contains(keyword)) ||
                (l.CargoType != null && l.CargoType.Contains(keyword)));
        }

        // ── Price range ────────────────────────────────────────────────────────
        if (search.MinPrice.HasValue)
            query = query.Where(l => l.Price >= search.MinPrice.Value);

        if (search.MaxPrice.HasValue)
            query = query.Where(l => l.Price <= search.MaxPrice.Value);

        // ── Availability window ────────────────────────────────────────────────
        if (search.AvailableFrom.HasValue)
            query = query.Where(l =>
                l.AvailableFrom == null ||
                l.AvailableFrom >= search.AvailableFrom.Value);

        if (search.AvailableTo.HasValue)
            query = query.Where(l =>
                l.AvailableFrom == null ||
                l.AvailableFrom <= search.AvailableTo.Value);

        // ── Exclude expired listings ───────────────────────────────────────────
        // Even if status is Active, an expired listing should not appear
        query = query.Where(l =>
            l.ExpiresAt == null || l.ExpiresAt > DateTime.UtcNow);

        return query;
    }

    // ── Private: Apply sorting ─────────────────────────────────────────────────

    private static IQueryable<Listing> ApplySorting(
        IQueryable<Listing> query, ListingSortBy sortBy) =>
        sortBy switch
        {
            ListingSortBy.Newest => query.OrderByDescending(l => l.CreatedAt),
            ListingSortBy.Oldest => query.OrderBy(l => l.CreatedAt),
            ListingSortBy.PriceAsc => query.OrderBy(l => l.Price)
                                              .ThenByDescending(l => l.CreatedAt),
            ListingSortBy.PriceDesc => query.OrderByDescending(l => l.Price)
                                              .ThenByDescending(l => l.CreatedAt),
            ListingSortBy.ExpiresFirst => query.OrderBy(l => l.ExpiresAt == null)
                                              .ThenBy(l => l.ExpiresAt)
                                              .ThenByDescending(l => l.CreatedAt),
            _ => query.OrderByDescending(l => l.CreatedAt)
        };
}