using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Data;
using SaaS.Modules.Jobs.Entities;
using SaaS.Modules.Jobs.DTOs;
using SaaS.Modules.Jobs.Repositories;
using SaaS.Shared;

namespace SaaS.Infrastructure.Repositories;

public sealed class JobRepository : IJobRepository
{
    private readonly AppDbContext db;
    public JobRepository(AppDbContext db) => this.db = db;

    private IQueryable<JobListing> BaseQuery => db.Set<JobListing>().AsNoTracking();

    public async Task<PagedResult<JobListing>> GetFilteredAsync(JobFilterDto filter, CancellationToken ct = default)
    {
        var query = BaseQuery;

        if (!string.IsNullOrWhiteSpace(filter.Company))
            query = query.Where(j => j.Company == filter.Company.Trim());

        if (!string.IsNullOrWhiteSpace(filter.Province))
            query = query.Where(j => j.Province == filter.Province.Trim());

        if (!string.IsNullOrWhiteSpace(filter.PostKind))
            query = query.Where(j => j.PostKind == filter.PostKind.Trim());

        if (!string.IsNullOrWhiteSpace(filter.EmploymentType))
            query = query.Where(j => j.EmploymentType == filter.EmploymentType.Trim());

        query = query.OrderByDescending(j => j.MembershipTier != "Free").ThenByDescending(j => j.CreatedAt);

        var total = await query.CountAsync(ct);
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 200);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<JobListing>(items, total, page, pageSize);
    }

    public async Task AddAsync(JobListing job, CancellationToken ct = default) => await db.Set<JobListing>().AddAsync(job, ct);

    public async Task<JobListing?> GetByIdAsync(Guid id, CancellationToken ct = default) => await db.Set<JobListing>().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) => await db.SaveChangesAsync(ct);
}
