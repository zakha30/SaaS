using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Data;
using SaaS.Modules.Fleet.Entities;
using SaaS.Modules.Fleet.Repositories;
using SaaS.Shared;

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
}
