using SaaS.Modules.Drivers.DTOs;
using SaaS.Modules.Drivers.Entities;
using SaaS.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaaS.Modules.Drivers.Repositories
{
    public interface IDriverRepository
    {
        Task<Driver?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<PagedResult<Driver>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<PagedResult<Driver>> GetFilteredAsync(DriverFilterDto filter, CancellationToken ct = default);
        Task AddAsync(Driver driver, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
        Task<bool> ExistsByEmailAsync(string email, Guid? excludeId, CancellationToken ct = default);
    }
}
