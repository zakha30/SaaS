using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Data;
using SaaS.Modules.Forum.Entities;
using SaaS.Modules.Forum.DTOs;
using SaaS.Modules.Forum.Repositories;
using SaaS.Shared;

namespace SaaS.Infrastructure.Repositories;

public sealed class ForumRepository : IThreadRepository
{
    private readonly AppDbContext db;
    public ForumRepository(AppDbContext db) => this.db = db;

    private IQueryable<ForumThread> BaseQuery => db.Set<ForumThread>().AsNoTracking();

    public async Task<PagedResult<ForumThread>> GetFilteredAsync(ThreadFilterDto filter, CancellationToken ct = default)
    {
        var query = BaseQuery;

        if (!string.IsNullOrWhiteSpace(filter.Category))
            query = query.Where(t => t.Category == filter.Category.Trim());

        query = query.OrderByDescending(t => t.MembershipTier != "Free").ThenByDescending(t => t.CreatedAt);

        var total = await query.CountAsync(ct);
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 200);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<ForumThread>(items, total, page, pageSize);
    }

    public async Task AddAsync(ForumThread thread, CancellationToken ct = default) => await db.Set<ForumThread>().AddAsync(thread, ct);

    public async Task<ForumThread?> GetByIdAsync(Guid id, CancellationToken ct = default) => await db.Set<ForumThread>().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) => await db.SaveChangesAsync(ct);
}
