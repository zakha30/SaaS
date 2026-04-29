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

public sealed class TenantRepository(AppDbContext db) : ITenantRepository
{
    // Tenants table has no TenantId filter — it is the global table.
    // IgnoreQueryFilters() is NOT needed here because Tenant uses BaseEntity,
    // not TenantEntity, so no global filter is applied to it.

    public async Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Tenants
                .Include(t => t.Settings)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct);

    public async Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct = default) =>
        await db.Tenants
                .FirstOrDefaultAsync(t => t.Slug == slug && !t.IsDeleted, ct);

    public async Task<IReadOnlyList<Tenant>> GetAllAsync(CancellationToken ct = default) =>
        await db.Tenants
                .AsNoTracking()
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.Name)
                .ToListAsync(ct);

    public async Task AddAsync(Tenant tenant, CancellationToken ct = default) =>
        await db.Tenants.AddAsync(tenant, ct);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
        await db.Tenants.AnyAsync(t => t.Id == id && !t.IsDeleted, ct);

    public async Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default) =>
        await db.Tenants.AnyAsync(t => t.Slug == slug, ct);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        await db.Tenants.AnyAsync(t => t.ContactEmail == email && !t.IsDeleted, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);
}