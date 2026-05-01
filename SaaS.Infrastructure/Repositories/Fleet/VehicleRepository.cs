using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Data;
using SaaS.Infrastructure.Modules.Fleet.DTOs;
using SaaS.Infrastructure.Modules.Fleet.Entities;
using SaaS.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaaS.Infrastructure.Repositories.Fleet;

public sealed class VehicleRepository(AppDbContext db) : IVehicleRepository
{
    private IQueryable<Vehicle> BaseQuery => db.Set<Vehicle>().AsNoTracking();

    public async Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Set<Vehicle>().FirstOrDefaultAsync(v => v.Id == id, ct);

    public async Task<PagedResult<Vehicle>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var query = BaseQuery.OrderByDescending(v => v.CreatedAt);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<Vehicle>(items, total, page, pageSize);
    }

    public async Task AddAsync(Vehicle vehicle, CancellationToken ct = default) =>
        await db.Set<Vehicle>().AddAsync(vehicle, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);

    public async Task<PagedResult<Vehicle>> GetFilteredAsync(Modules.Fleet.DTOs.VehicleFilterDto filter, CancellationToken ct = default)
    {
        var query = BaseQuery;

        if (filter.Category.HasValue)
        {
            query = query.Where(v => v.Category == filter.Category.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Province))
        {
            var prov = filter.Province.Trim();
            query = query.Where(v => v.Province == prov);
        }

        if (!string.IsNullOrWhiteSpace(filter.TruckType))
        {
            var tt = filter.TruckType.Trim();
            query = query.Where(v => v.TruckType == tt);
        }

        if (filter.IsCrossBorderCapable.HasValue)
        {
            query = query.Where(v => v.IsCrossBorderCapable == filter.IsCrossBorderCapable.Value);
        }

        if (filter.MinPayloadTons.HasValue)
        {
            query = query.Where(v => v.PayloadTons >= filter.MinPayloadTons.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            var st = filter.Status.Trim();
            query = query.Where(v => v.Status == st);
        }

        // Members first (MembershipTier != Free), then newest
        query = query.OrderByDescending(v => v.MembershipTier != MembershipTierConstants.Free)
                     .ThenByDescending(v => v.CreatedAt);

        var total = await query.CountAsync(ct);
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 200);

        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<Vehicle>(items, total, page, pageSize);
    }
}

public sealed class FleetImageRepository(AppDbContext db) : IFleetImageRepository
{
    private IQueryable<FleetImage> BaseQuery => db.Set<FleetImage>().AsNoTracking();

    public async Task<FleetImage?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await db.Set<FleetImage>().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<List<FleetImage>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken ct = default) =>  // ← renamed
        await BaseQuery
            .Where(x => x.VehicleId == vehicleId)  // ← changed from FleetId
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(FleetImage image, CancellationToken ct = default) =>
        await db.Set<FleetImage>().AddAsync(image, ct);

    public async Task DeleteAsync(FleetImage image, CancellationToken ct = default)
    {
        db.Set<FleetImage>().Remove(image);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);
}
