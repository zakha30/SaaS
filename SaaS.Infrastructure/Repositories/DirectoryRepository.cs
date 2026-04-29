using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Data;
using SaaS.Modules.Directory.Entities;
using SaaS.Modules.Directory.DTOs;
using SaaS.Modules.Directory.Repositories;
using SaaS.Shared;

namespace SaaS.Infrastructure.Repositories;

public sealed class DirectoryRepository : IDirectoryRepository
{
    private readonly AppDbContext db;
    public DirectoryRepository(AppDbContext db) => this.db = db;

    private IQueryable<BusinessDirectoryEntry> BaseQuery => db.Set<BusinessDirectoryEntry>().AsNoTracking();

    public async Task<PagedResult<BusinessDirectoryEntry>> GetFilteredAsync(DirectoryFilterDto filter, CancellationToken ct = default)
    {
        var query = BaseQuery;

        if (!string.IsNullOrWhiteSpace(filter.Category))
            query = query.Where(d => d.Category == filter.Category.Trim());

        if (!string.IsNullOrWhiteSpace(filter.Province))
            query = query.Where(d => d.Province == filter.Province.Trim());

        if (!string.IsNullOrWhiteSpace(filter.Slug))
            query = query.Where(d => d.Slug == filter.Slug.Trim());

        query = query.OrderByDescending(d => d.MembershipTier != "Free").ThenByDescending(d => d.CreatedAt);

        var total = await query.CountAsync(ct);
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 200);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<BusinessDirectoryEntry>(items, total, page, pageSize);
    }

    public async Task AddAsync(BusinessDirectoryEntry entry, CancellationToken ct = default) => await db.Set<BusinessDirectoryEntry>().AddAsync(entry, ct);

    public async Task<BusinessDirectoryEntry?> GetByIdAsync(Guid id, CancellationToken ct = default) => await db.Set<BusinessDirectoryEntry>().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) => await db.SaveChangesAsync(ct);
}
