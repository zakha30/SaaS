using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Data;
using SaaS.Modules.Classifieds.Entities;
using SaaS.Modules.Classifieds.DTOs;
using SaaS.Modules.Classifieds.Repositories;
using SaaS.Shared;

namespace SaaS.Infrastructure.Repositories;

public sealed class ClassifiedRepository : IClassifiedRepository
{
    private readonly AppDbContext db;
    public ClassifiedRepository(AppDbContext db) => this.db = db;

    private IQueryable<ClassifiedItem> BaseQuery => db.Set<ClassifiedItem>().AsNoTracking();

    public async Task<PagedResult<ClassifiedItem>> GetFilteredAsync(ClassifiedFilterDto filter, CancellationToken ct = default)
    {
        var query = BaseQuery;

        if (!string.IsNullOrWhiteSpace(filter.Category))
            query = query.Where(c => c.Category == filter.Category.Trim());

        if (!string.IsNullOrWhiteSpace(filter.Province))
            query = query.Where(c => c.Province == filter.Province.Trim());

        if (!string.IsNullOrWhiteSpace(filter.TradeKind))
            query = query.Where(c => c.TradeKind == filter.TradeKind.Trim());

        query = query.OrderByDescending(c => c.MembershipTier != "Free").ThenByDescending(c => c.CreatedAt);

        var total = await query.CountAsync(ct);
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 200);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<ClassifiedItem>(items, total, page, pageSize);
    }

    public async Task AddAsync(ClassifiedItem item, CancellationToken ct = default) => await db.Set<ClassifiedItem>().AddAsync(item, ct);

    public async Task<ClassifiedItem?> GetByIdAsync(Guid id, CancellationToken ct = default) => await db.Set<ClassifiedItem>().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) => await db.SaveChangesAsync(ct);
}
