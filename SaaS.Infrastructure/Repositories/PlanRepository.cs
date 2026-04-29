using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Data;
using SaaS.Modules.Tenants.Entities;
using SaaS.Modules.Tenants.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaaS.Infrastructure.Repositories;

public sealed class PlanRepository(AppDbContext db) : IPlanRepository
{
    public async Task<Plan?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Plans.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Plan>> GetAllActiveAsync(CancellationToken ct = default) =>
        await db.Plans.AsNoTracking().Where(p => p.IsActive).ToListAsync(ct);

    public async Task AddAsync(Plan plan, CancellationToken ct = default) =>
        await db.Plans.AddAsync(plan, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);
}
