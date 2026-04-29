using System.Threading;
using System.Threading.Tasks;
using SaaS.Modules.Loads.Entities;
using SaaS.Shared;

namespace SaaS.Modules.Loads.Repositories;

public interface IAvailableLoadRepository
{
    Task<PagedResult<AvailableLoad>> GetFilteredAsync(DTOs.LoadFilterDto filter, CancellationToken ct = default);
    Task AddAsync(AvailableLoad load, CancellationToken ct = default);
    Task<AvailableLoad?> GetByIdAsync(System.Guid id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
