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
using SaaS.Modules.Drivers.Repositories;
using SaaS.Modules.Drivers.Entities;
using SaaS.Modules.Drivers.DTOs;

namespace SaaS.Infrastructure.Repositories;

    public sealed class DriverRepository(AppDbContext db) : IDriverRepository
    {
        private IQueryable<Driver> BaseQuery => db.Set<Driver>().AsNoTracking();

        public async Task<Driver?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            await db.Set<Driver>().FirstOrDefaultAsync(d => d.Id == id, ct);

        public async Task<PagedResult<Driver>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var query = BaseQuery.OrderByDescending(d => d.CreatedAt);
            var total = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return new PagedResult<Driver>(items, total, page, pageSize);
        }

        public async Task<PagedResult<Driver>> GetFilteredAsync(DriverFilterDto filter, CancellationToken ct = default)
        {
            var query = BaseQuery;

            if (!string.IsNullOrWhiteSpace(filter.Status))
                query = query.Where(d => d.Status == filter.Status.Trim());

            if (!string.IsNullOrWhiteSpace(filter.Region))
                query = query.Where(d => d.Region == filter.Region.Trim());

            query = query.OrderByDescending(d => d.CreatedAt);

            var total = await query.CountAsync(ct);
            var page = Math.Max(1, filter.Page);
            var pageSize = Math.Clamp(filter.PageSize, 1, 100);

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return new PagedResult<Driver>(items, total, page, pageSize);
        }

        public async Task AddAsync(Driver driver, CancellationToken ct = default) =>
            await db.Set<Driver>().AddAsync(driver, ct);

        public async Task SaveChangesAsync(CancellationToken ct = default) =>
            await db.SaveChangesAsync(ct);

        public async Task<bool> ExistsByEmailAsync(string email, Guid? excludeId, CancellationToken ct = default) =>
            await BaseQuery.AnyAsync(d =>
                d.Email == email && (excludeId == null || d.Id != excludeId.Value), ct);
    }



