using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Data;
using SaaS.Modules.Loads.Entities;
using SaaS.Modules.Loads.DTOs;
using SaaS.Modules.Loads.Repositories;
using SaaS.Shared;

namespace SaaS.Infrastructure.Repositories;

public sealed class AvailableLoadRepository : IAvailableLoadRepository
{
    private readonly AppDbContext db;
    public AvailableLoadRepository(AppDbContext db) => this.db = db;

    private IQueryable<AvailableLoad> BaseQuery => db.Set<AvailableLoad>().AsNoTracking();

    public async Task<PagedResult<AvailableLoad>> GetFilteredAsync(LoadFilterDto filter, CancellationToken ct = default)
    {
        var query = BaseQuery;

        if (!string.IsNullOrWhiteSpace(filter.DepartureProvince))
            query = query.Where(l => l.DepartureProvince == filter.DepartureProvince.Trim());

        if (!string.IsNullOrWhiteSpace(filter.DestinationProvince))
            query = query.Where(l => l.DestinationProvince == filter.DestinationProvince.Trim());

        if (!string.IsNullOrWhiteSpace(filter.TruckTypeRequired))
            query = query.Where(l => l.TruckTypeRequired == filter.TruckTypeRequired.Trim());

        if (filter.IsCrossBorder.HasValue)
            query = query.Where(l => l.IsCrossBorder == filter.IsCrossBorder.Value);

        if (filter.MinWeightKg.HasValue)
            query = query.Where(l => l.WeightKg >= filter.MinWeightKg.Value);

        if (!string.IsNullOrWhiteSpace(filter.Status))
            query = query.Where(l => l.Status == filter.Status.Trim());

        if (!string.IsNullOrWhiteSpace(filter.Commodity))
        {
            var term = filter.Commodity.Trim();
            query = query.Where(l => l.Commodity.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(filter.WeightBracket))
        {
            var w = filter.WeightBracket.Trim();
            query = query.Where(l => l.WeightBracket != null && l.WeightBracket.Contains(w));
        }

        query = query.OrderByDescending(l => l.MembershipTier != "Free").ThenByDescending(l => l.CreatedAt);

        var total = await query.CountAsync(ct);
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 200);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<AvailableLoad>(items, total, page, pageSize);
    }

    public async Task AddAsync(AvailableLoad load, CancellationToken ct = default) => await db.Set<AvailableLoad>().AddAsync(load, ct);

    public async Task<AvailableLoad?> GetByIdAsync(Guid id, CancellationToken ct = default) => await db.Set<AvailableLoad>().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) => await db.SaveChangesAsync(ct);
}
